using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Guid("3E22D313-F08B-41A5-86C8-95E95CB49FFC")]
    [ComConversionLoss]
    [InterfaceType(1)]
    [ComImport]
    internal interface IOPCItemSamplingMgt
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetItemSamplingRate(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray), In] int[] pdwRequestedSamplingRate,
            out IntPtr ppdwRevisedSamplingRate,
            out IntPtr ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetItemSamplingRate(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            out IntPtr ppdwSamplingRate,
            out IntPtr ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearItemSamplingRate(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            out IntPtr ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetItemBufferEnable(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray), In] int[] pbEnable,
            out IntPtr ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetItemBufferEnable(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            out IntPtr ppbEnable,
            out IntPtr ppErrors);
    }
}