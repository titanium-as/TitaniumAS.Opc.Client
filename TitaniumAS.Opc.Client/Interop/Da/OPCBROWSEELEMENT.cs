using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCBROWSEELEMENT
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string szName;
        [MarshalAs(UnmanagedType.LPWStr)] public string szItemID;
        public int dwFlagValue;
        public int dwReserved;
        public OPCITEMPROPERTIES ItemProperties;
    }
}