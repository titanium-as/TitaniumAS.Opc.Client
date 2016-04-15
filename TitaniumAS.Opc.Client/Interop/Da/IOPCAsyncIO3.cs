using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [Guid("0967B97B-36EF-423E-B6F8-6BFF1E40D39D")]
    [InterfaceType(1)]
    [ComImport]
    internal interface IOPCAsyncIO3 : IOPCAsyncIO2
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
        void SetEnable([In] int bEnable);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEnable(out int pbEnable);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ReadMaxAge([In] int dwCount, [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray), In] int[] pdwMaxAge, [In] int dwTransactionID, out int pdwCancelID,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WriteVQT([In] int dwCount, [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] OPCITEMVQT[] pItemVQT,
            [In] int dwTransactionID, out int pdwCancelID,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RefreshMaxAge([In] int dwMaxAge, [In] int dwTransactionID, out int pdwCancelID);
    }
}