using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [InterfaceType(1)]
    [Guid("39C13A71-011E-11D0-9675-0020AFD8ADB3")]
    [ComImport]
    internal interface IOPCAsyncIO2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Read(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [In] int dwTransactionID,
            out int pdwCancelID,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Write(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] object[] pItemValues,
            [In] int dwTransactionID, out int pdwCancelID,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Refresh2([In] OPCDATASOURCE dwSource, [In] int dwTransactionID, out int pdwCancelID);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Cancel2([In] int dwCancelID);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetEnable([MarshalAs(UnmanagedType.Bool), In] bool bEnable);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEnable([MarshalAs(UnmanagedType.Bool), Out] out bool pbEnable);
    }
}