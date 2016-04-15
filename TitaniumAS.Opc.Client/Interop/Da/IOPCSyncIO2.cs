using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Guid("730F5F0F-55B1-4C81-9E18-FF8A0904E1FA")]
    [InterfaceType(1)]
    [ComImport]
    internal interface IOPCSyncIO2 : IOPCSyncIO
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
            [MarshalAs(UnmanagedType.LPArray), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct), In] object[] pItemValues,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1), Out] out
                HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ReadMaxAge([In] int dwCount, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), In] int[] phServer, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), In] int[] pdwMaxAge, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out object[] ppvValues, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), Out] out short[] ppwQualities, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out FILETIME[] ppftTimeStamps, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void WriteVQT(
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0), In] int[] phServer,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), In] OPCITEMVQT[]
                pItemVQT,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 0), Out] out
                HRESULT[] ppErrors);
    }
}