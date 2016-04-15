using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [InterfaceType(1)]
    [ComConversionLoss]
    [Guid("39C13A53-011E-11D0-9675-0020AFD8ADB3")]
    [ComImport]
    internal interface IOPCAsyncIO
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Read([In] int dwConnection, [In] OPCDATASOURCE dwSource, [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] int[] phServer, out int pTransactionID,
            out IntPtr ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Write([In] int dwConnection, [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1, ArraySubType = UnmanagedType.Struct), In] object[]
                pItemValues, out int pTransactionID, out IntPtr ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Refresh([In] int dwConnection, [In] OPCDATASOURCE dwSource, out int pTransactionID);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Cancel([In] int dwTransactionID);
    }
}