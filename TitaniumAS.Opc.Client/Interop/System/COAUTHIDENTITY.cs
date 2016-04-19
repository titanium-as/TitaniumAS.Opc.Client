using System;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.System
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct COAUTHIDENTITY
    {
        public IntPtr User;
        public uint UserLength;
        public IntPtr Domain;
        public uint DomainLength;
        public IntPtr Password;
        public uint PasswordLength;
        public uint Flags;
    }
}