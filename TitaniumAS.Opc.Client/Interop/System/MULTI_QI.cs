using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MULTI_QI
    {
        public IntPtr iid;
        [MarshalAs(UnmanagedType.IUnknown)] public object pItf;
        public uint hr;
    }
}