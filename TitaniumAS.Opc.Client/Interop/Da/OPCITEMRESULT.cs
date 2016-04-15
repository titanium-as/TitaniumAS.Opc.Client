using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    //[ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCITEMRESULT
    {
        public int hServer;
        public short vtCanonicalDataType;
        public short wReserved;
        public int dwAccessRights;
        public int dwBlobSize;
        //[ComConversionLoss]
        //public IntPtr pBlob;
        public byte[] pBlob;
    }
}