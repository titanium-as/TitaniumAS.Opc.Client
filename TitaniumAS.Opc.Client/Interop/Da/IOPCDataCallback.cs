using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [InterfaceType(1)]
    [Guid("39C13A70-011E-11D0-9675-0020AFD8ADB3")]
    [ComImport]
    internal interface IOPCDataCallback
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnDataChange(
            [In] int dwTransid,
            [In] int hGroup,
            [In] HRESULT hrMasterquality,
            [In] HRESULT hrMastererror,
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4), In] int[] phClientItems,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.Struct), In] object[]
                pvValues,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4), In] short[] pwQualities,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.Struct), In] FILETIME[]
                pftTimeStamps,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 4), In] HRESULT[]
                pErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnReadComplete(
            [In] int dwTransid,
            [In] int hGroup,
            [In] HRESULT hrMasterquality,
            [In] HRESULT hrMastererror,
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4), In] int[] phClientItems,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.Struct), In] object[]
                pvValues,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4), In] short[] pwQualities,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.Struct), In] FILETIME[]
                pftTimeStamps,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 4), In] HRESULT[]
                pErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnWriteComplete(
            [In] int dwTransid,
            [In] int hGroup,
            [In] HRESULT hrMastererr,
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), In] int[] pClienthandles,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 3), In] HRESULT[]
                pErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnCancelComplete([In] int dwTransid, [In] int hGroup);
    }
}