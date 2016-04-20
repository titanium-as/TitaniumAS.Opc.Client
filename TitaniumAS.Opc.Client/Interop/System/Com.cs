using System;
using System.Net;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.System
{
    internal static class Com
    {
        public static readonly Guid IUnknownIID = new Guid("00000000-0000-0000-C000-000000000046");

        [DllImport("ole32.dll")]
        private static extern int CoInitializeSecurity(
            IntPtr pSecDesc,
            int cAuthSvc,
            SOLE_AUTHENTICATION_SERVICE[] asAuthSvc,
            IntPtr pReserved1,
            uint dwAuthnLevel,
            uint dwImpLevel,
            IntPtr pAuthList,
            uint dwCapabilities,
            IntPtr pReserved3);

        [DllImport("ole32.dll")]
        private static extern void CoCreateInstanceEx(
            ref Guid clsid,
            [MarshalAs(UnmanagedType.IUnknown)] object punkOuter,
            uint dwClsCtx,
            [In] ref COSERVERINFO pServerInfo,
            uint dwCount,
            [In, Out] MULTI_QI[] pResults);

        [DllImport("ole32.dll")]
        private static extern void CoGetClassObject(
            [MarshalAs(UnmanagedType.LPStruct)] Guid clsid,
            uint dwClsContext,
            [In] ref COSERVERINFO pServerInfo,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] [Out] out object ppv);

        [DllImport("ole32.dll")]
        private static extern int CoSetProxyBlanket(
            [MarshalAs(UnmanagedType.IUnknown)] object pProxy, uint dwAuthnSvc, uint dwAuthzSvc,
            [MarshalAs(UnmanagedType.LPWStr)] string pServerPrincName, uint dwAuthnLevel,
            uint dwImpLevel, IntPtr pAuthInfo, uint dwCapabilities);

        /// <summary>
        /// Initializes COM security.
        /// </summary>
        public static void InitializeSecurity()
        {
            int error = CoInitializeSecurity(
                IntPtr.Zero,
                -1,
                null,
                IntPtr.Zero,
                ComConstants.RPC_C_AUTHN_LEVEL_NONE,
                ComConstants.RPC_C_IMP_LEVEL_IDENTIFY,
                IntPtr.Zero,
                ComConstants.EOAC_NONE,
                IntPtr.Zero);

            if (error != 0)
            {
                throw new ExternalException("CoInitializeSecurity: " + GetSystemMessage(error), error);
            }
        }

        public static void SetProxyBlanket(object comObject, ComProxyBlanket comProxyBlanket = null)
        {
            if (comProxyBlanket != null)
            {
                var result =
                    new HRESULT(CoSetProxyBlanket(comObject, (uint) comProxyBlanket.RpcAuthService,
                        (uint) comProxyBlanket.RpcAuthType,
                        null, (uint) comProxyBlanket.RpcAuthnLevel, (uint) comProxyBlanket.RpcImpLevel, IntPtr.Zero,
                        (uint) comProxyBlanket.DwCapabilities));
                if (result.Failed)
                {
                    throw HRESULT.GetExceptionForHR(result);
                }
            }
        }

        public static object CreateInstance(Guid clsid, string host, NetworkCredential credential)
        {
            /*var serverType = Type.GetTypeFromCLSID(clsid, host);
            var comObject = Activator.CreateInstance(serverType);
            return comObject;*/

            var serverInfo = new ServerInfo();
            COSERVERINFO coserverInfo = serverInfo.Allocate(host, credential);

            GCHandle hIID = GCHandle.Alloc(IUnknownIID, GCHandleType.Pinned);

            var results = new MULTI_QI[1];

            results[0].iid = hIID.AddrOfPinnedObject();
            results[0].pItf = null;
            results[0].hr = 0;

            try
            {
                // Check whether connecting locally or remotely.
                uint clsctx = ComConstants.CLSCTX_INPROC_SERVER | ComConstants.CLSCTX_LOCAL_SERVER;

                if (!string.IsNullOrEmpty(host) && host != "localhost")
                {
                    clsctx = ComConstants.CLSCTX_LOCAL_SERVER | ComConstants.CLSCTX_REMOTE_SERVER;
                }

                // Create an instance.
                CoCreateInstanceEx(
                    ref clsid,
                    null,
                    clsctx,
                    ref coserverInfo,
                    1,
                    results);
            }
            finally
            {
                if (hIID.IsAllocated) hIID.Free();
                serverInfo.Deallocate();
            }

            int error = (int) results[0].hr;
            if (error != 0)
            {
                throw new ExternalException("CoCreateInstanceEx: " + GetSystemMessage(error), error);
            }

            return results[0].pItf;
        }

        /// <summary>
        /// Creates an instance of a COM server and call CoSetProxyBlanket.
        /// </summary>
        public static object CreateInstanceWithBlanket(Guid clsid, string host, NetworkCredential credential,
            ComProxyBlanket comProxyBlanket = null)
        {
            object obj = CreateInstance(clsid, host, credential);
            SetProxyBlanket(obj, comProxyBlanket);
            return obj;
        }

        /// <summary>
        /// Creates an instance of a COM server using the specified license key and call CoSetProxyBlanket.
        /// </summary>
        public static object CreateInstanceWithLicenseKeyAndBlanket(Guid clsid, string host,
            NetworkCredential credential, string licenseKey, ComProxyBlanket comProxyBlanket = null)
        {
            object obj = CreateInstanceWithLicenseKey(clsid, host, credential, licenseKey);
            SetProxyBlanket(obj, comProxyBlanket);
            return obj;
        }

        /// <summary>
        /// Creates an instance of a COM server using the specified license key.
        /// </summary>
        public static object CreateInstanceWithLicenseKey(Guid clsid, string hostName, NetworkCredential credential, string licenseKey)
        {
            var serverInfo = new ServerInfo();
            COSERVERINFO coserverInfo = serverInfo.Allocate(hostName, credential);
            object instance = null;
            IClassFactory2 factory = null;

            try
            {
                // Check whether connecting locally or remotely.
                uint clsctx = ComConstants.CLSCTX_INPROC_SERVER | ComConstants.CLSCTX_LOCAL_SERVER;

                if (!string.IsNullOrEmpty(hostName))
                {
                    clsctx = ComConstants.CLSCTX_LOCAL_SERVER | ComConstants.CLSCTX_REMOTE_SERVER;
                }

                // Get the class factory.
                object unknown = null;

                CoGetClassObject(
                    clsid,
                    clsctx,
                    ref coserverInfo,
                    typeof (IClassFactory2).GUID,
                    out unknown);

                factory = (IClassFactory2) unknown;

                // Set the proper connect authentication level
                var security = (IClientSecurity) factory;

                uint pAuthnSvc = 0;
                uint pAuthzSvc = 0;
                var pServerPrincName = "";
                uint pAuthnLevel = 0;
                uint pImpLevel = 0;
                IntPtr pAuthInfo = IntPtr.Zero;
                uint pCapabilities = 0;

                // Get existing security settings.
                security.QueryBlanket(
                    factory,
                    ref pAuthnSvc,
                    ref pAuthzSvc,
                    ref pServerPrincName,
                    ref pAuthnLevel,
                    ref pImpLevel,
                    ref pAuthInfo,
                    ref pCapabilities);

                pAuthnSvc = ComConstants.RPC_C_AUTHN_DEFAULT;
                pAuthnLevel = ComConstants.RPC_C_AUTHN_LEVEL_CONNECT;

                // Update security settings.
                security.SetBlanket(
                    factory,
                    pAuthnSvc,
                    pAuthzSvc,
                    pServerPrincName,
                    pAuthnLevel,
                    pImpLevel,
                    pAuthInfo,
                    pCapabilities);

                // Create instance.
                factory.CreateInstanceLic(
                    null,
                    null,
                    IUnknownIID,
                    licenseKey,
                    out instance);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                serverInfo.Deallocate();
            }

            return instance;
        }

        /// <summary>
        /// Releases the server if it is a true COM server.
        /// </summary>
        public static void ReleaseComServer(this object server)
        {
            if (server != null && server.GetType().IsCOMObject)
            {
                Marshal.ReleaseComObject(server);
            }
        }

        [DllImport("Kernel32.dll")]
        private static extern int FormatMessageW(
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            IntPtr lpBuffer,
            int nSize,
            IntPtr Arguments);

        /// <summary>
        /// Retrieves the system message text for the specified error.
        /// </summary>
        public static string GetSystemMessage(int error)
        {
            const int MAX_MESSAGE_LENGTH = 1024;
            const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

            IntPtr buffer = Marshal.AllocCoTaskMem(MAX_MESSAGE_LENGTH);

            int result = FormatMessageW(
                (int) (FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM),
                IntPtr.Zero,
                error,
                0,
                buffer,
                MAX_MESSAGE_LENGTH - 1,
                IntPtr.Zero);

            string msg = Marshal.PtrToStringUni(buffer);
            Marshal.FreeCoTaskMem(buffer);

            if (!string.IsNullOrEmpty(msg))
            {
                return msg;
            }

            return string.Format("0x{0,0:X}", error);
        }

        public static T QueryInterface<T>(this object comServer) where T : class
        {
            return (T) comServer;
        }

        public static T TryQueryInterface<T>(this object comServer) where T : class
        {
            return comServer as T;
        }
    }
}