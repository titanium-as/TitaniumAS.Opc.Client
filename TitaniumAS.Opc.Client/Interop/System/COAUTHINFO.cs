using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct COAUTHINFO
    {
        public uint dwAuthnSvc;
        public uint dwAuthzSvc;
        public IntPtr pwszServerPrincName;
        public uint dwAuthnLevel;
        public uint dwImpersonationLevel;
        public IntPtr pAuthIdentityData;
        public uint dwCapabilities;
    }
}