using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct OPCITEMSTATE
    {
        public int hClient;
        public FILETIME ftTimeStamp;
        public short wQuality;
        public short wReserved;
        [MarshalAs(UnmanagedType.Struct)] public object vDataValue;
    }
}