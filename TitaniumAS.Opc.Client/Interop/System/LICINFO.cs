using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct LICINFO
    {
        public readonly int cbLicInfo;
        [MarshalAs(UnmanagedType.Bool)] public readonly bool fRuntimeKeyAvail;
        [MarshalAs(UnmanagedType.Bool)] public readonly bool fLicVerified;
    }
}