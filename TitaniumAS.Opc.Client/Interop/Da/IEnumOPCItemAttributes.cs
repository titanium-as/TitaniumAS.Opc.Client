using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [InterfaceType(1)]
    [Guid("39C13A55-011E-11D0-9675-0020AFD8ADB3")]
    [ComConversionLoss]
    [ComImport]
    internal interface IEnumOPCItemAttributes
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Next([In] int celt, out IntPtr ppItemArray, out int pceltFetched);

        //void Next([In] int celt, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), Out] out OPCITEMATTRIBUTES[] ppItemArray, out int pceltFetched);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Skip([In] int celt);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Reset();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumOPCItemAttributes ppEnumItemAttributes);
    }
}