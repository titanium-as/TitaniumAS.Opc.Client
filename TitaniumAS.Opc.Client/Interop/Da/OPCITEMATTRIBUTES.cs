using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCITEMATTRIBUTES
    {
        [MarshalAs(UnmanagedType.LPWStr)] public string szAccessPath;
        [MarshalAs(UnmanagedType.LPWStr)] public string szItemID;
        [MarshalAs(UnmanagedType.Bool)] public bool bActive;
        public int hClient;
        public int hServer;
        public int dwAccessRights;
        public int dwBlobSize;
        [ComConversionLoss] public IntPtr pBlob;
        public short vtRequestedDataType;
        public short vtCanonicalDataType;
        public OPCEUTYPE dwEUType;
        [MarshalAs(UnmanagedType.Struct)] public object vEUInfo;
    }
}