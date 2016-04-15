using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [InterfaceType(1)]
    [Guid("39C13A54-011E-11D0-9675-0020AFD8ADB3")]
    [ComConversionLoss]
    [ComImport]
    internal interface IOPCItemMgt
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddItems(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] OPCITEMDEF[] pItemArray,
            out IntPtr ppAddResults,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ValidateItems(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] OPCITEMDEF[] pItemArray,
            [MarshalAs(UnmanagedType.Bool), In] bool bBlobUpdate,
            out IntPtr ppValidationResults,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveItems(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetActiveState(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.Bool)] [In] bool bActive,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientHandles(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phClient,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDatatypes(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray), In] short[] pRequestedDatatypes,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateEnumerator([In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }
}