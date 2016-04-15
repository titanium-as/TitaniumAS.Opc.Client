using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct OPCITEMPROPERTIES
    {
        public HRESULT hrErrorID;
        public int dwNumProperties;
        [ComConversionLoss] public IntPtr pItemProperties;
        public int dwReserved;
    }
}