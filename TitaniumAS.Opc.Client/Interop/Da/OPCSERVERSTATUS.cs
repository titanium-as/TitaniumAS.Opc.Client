using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    //[StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Unicode)]
    public struct OPCSERVERSTATUS
    {
        public FILETIME ftStartTime;
        public FILETIME ftCurrentTime;
        public FILETIME ftLastUpdateTime;
        [MarshalAs(UnmanagedType.U4)] public OPCSERVERSTATE dwServerState;
        public int dwGroupCount;
        public int dwBandWidth;
        public short wMajorVersion;
        public short wMinorVersion;
        public short wBuildNumber;
        public short wReserved;
        [MarshalAs(UnmanagedType.LPWStr)] public string szVendorInfo;
    }
}