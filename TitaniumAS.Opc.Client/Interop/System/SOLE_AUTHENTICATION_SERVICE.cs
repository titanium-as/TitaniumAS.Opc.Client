using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SOLE_AUTHENTICATION_SERVICE
    {
        public readonly uint dwAuthnSvc;
        public readonly uint dwAuthzSvc;
        [MarshalAs(UnmanagedType.LPWStr)] public readonly string pPrincipalName;
        public readonly int hr;
    }
}