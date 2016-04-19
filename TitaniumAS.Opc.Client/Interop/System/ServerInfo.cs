using System;
using System.Net;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    /// <summary>
    /// A class used to allocate and deallocate the elements of a COSERVERINFO structure.
    /// </summary>
    internal class ServerInfo
    {
        private GCHandle _hAuthInfo;
        private GCHandle _hDomain;
        private GCHandle _hIdentity;
        private GCHandle _hPassword;
        private GCHandle _hUserName;

        /// <summary>
        /// Allocates a COSERVERINFO structure.
        /// </summary>
        public COSERVERINFO Allocate(string hostName, NetworkCredential credential)
        {
            string userName = null;
            string password = null;
            string domain = null;

            if (credential != null)
            {
                userName = credential.UserName;
                password = credential.Password;
                domain = credential.Domain;
            }

            _hUserName = GCHandle.Alloc(userName, GCHandleType.Pinned);
            _hPassword = GCHandle.Alloc(password, GCHandleType.Pinned);
            _hDomain = GCHandle.Alloc(domain, GCHandleType.Pinned);

            _hIdentity = new GCHandle();

            if (!string.IsNullOrEmpty(userName))
            {
                var identity = new COAUTHIDENTITY
                {
                    User = _hUserName.AddrOfPinnedObject(),
                    UserLength = (uint) (userName != null ? userName.Length : 0),
                    Password = _hPassword.AddrOfPinnedObject(),
                    PasswordLength = (uint) (password != null ? password.Length : 0),
                    Domain = _hDomain.AddrOfPinnedObject(),
                    DomainLength = (uint) (domain != null ? domain.Length : 0),
                    Flags = ComConstants.SEC_WINNT_AUTH_IDENTITY_UNICODE
                };

                _hIdentity = GCHandle.Alloc(identity, GCHandleType.Pinned);
            }

            var authInfo = new COAUTHINFO
            {
                dwAuthnSvc = ComConstants.RPC_C_AUTHN_WINNT,
                dwAuthzSvc = ComConstants.RPC_C_AUTHZ_NONE,
                pwszServerPrincName = IntPtr.Zero,
                dwAuthnLevel = ComConstants.RPC_C_AUTHN_LEVEL_CONNECT,
                dwImpersonationLevel = ComConstants.RPC_C_IMP_LEVEL_IMPERSONATE,
                pAuthIdentityData = (_hIdentity.IsAllocated) ? _hIdentity.AddrOfPinnedObject() : IntPtr.Zero,
                dwCapabilities = ComConstants.EOAC_NONE
            };

            _hAuthInfo = GCHandle.Alloc(authInfo, GCHandleType.Pinned);

            var serverInfo = new COSERVERINFO
            {
                pwszName = hostName,
                pAuthInfo = credential != null ? _hAuthInfo.AddrOfPinnedObject() : IntPtr.Zero,
                dwReserved1 = 0,
                dwReserved2 = 0
            };

            return serverInfo;
        }

        /// <summary>
        /// Deallocated memory allocated when the COSERVERINFO structure was created.
        /// </summary>
        public void Deallocate()
        {
            if (_hUserName.IsAllocated)
            {
                _hUserName.Free();
            }

            if (_hPassword.IsAllocated)
            {
                _hPassword.Free();
            }

            if (_hDomain.IsAllocated)
            {
                _hDomain.Free();
            }

            if (_hIdentity.IsAllocated)
            {
                _hIdentity.Free();
            }

            if (_hAuthInfo.IsAllocated)
            {
                _hAuthInfo.Free();
            }
        }
    }
}