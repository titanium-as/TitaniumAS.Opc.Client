using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct OPCITEMVQT
    {
        [MarshalAs(UnmanagedType.Struct)] public object vDataValue;
        [MarshalAs(UnmanagedType.Bool)] public bool bQualitySpecified;
        public short wQuality;
        public short wReserved;
        [MarshalAs(UnmanagedType.Bool)] public bool bTimeStampSpecified;
        public int dwReserved;
        public FILETIME ftTimeStamp;
    }
}