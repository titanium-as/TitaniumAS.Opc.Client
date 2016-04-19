using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct COSERVERINFO
    {
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.LPWStr)] public string pwszName;
        public IntPtr pAuthInfo;
        public uint dwReserved2;
    };
}