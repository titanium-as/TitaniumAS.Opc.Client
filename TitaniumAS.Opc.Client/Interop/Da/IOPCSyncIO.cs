using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [InterfaceType(1)]
    [Guid("39C13A52-011E-11D0-9675-0020AFD8ADB3")]
    [ComImport]
    internal interface IOPCSyncIO
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Read(
            [In] OPCDATASOURCE dwSource,
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1), Out] out
                OPCITEMSTATE[] ppItemValues,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Write(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), In] object[]
                pItemValues,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);
    }
}