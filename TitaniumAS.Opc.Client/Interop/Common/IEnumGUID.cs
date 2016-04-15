using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Common
{
    [InterfaceType(1)]
    [Guid("0002E000-0000-0000-C000-000000000046")]
    [ComImport]
    interface IEnumGUID
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Next([In] int celt,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), Out] Guid[] rgelt,
            out int pceltFetched);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Skip([In] int celt);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Reset();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumGUID ppenum);
    }
}