using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCITEMDEF
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string szAccessPath;
        [MarshalAs(UnmanagedType.LPWStr)] public string szItemID;
        public int bActive;
        public int hClient;
        public int dwBlobSize;
        [ComConversionLoss] public IntPtr pBlob;
        public short vtRequestedDataType;
        public short wReserved;
    }
}