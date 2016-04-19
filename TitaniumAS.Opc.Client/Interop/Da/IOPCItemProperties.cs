using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComConversionLoss]
    [InterfaceType(1)]
    [Guid("39C13A72-011E-11D0-9675-0020AFD8ADB3")]
    [ComImport]
    interface IOPCItemProperties
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void QueryAvailableProperties(
            [MarshalAs(UnmanagedType.LPWStr), In] string szItemID, 
            out int pdwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out]out int[] ppPropertyIDs,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1), Out]out string[] ppDescriptions,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out]out int[] ppvtDataTypes);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetItemProperties(
            [MarshalAs(UnmanagedType.LPWStr), In] string szItemID, 
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In] int[] pdwPropertyIDs,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1), Out]out object[] ppvData,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1), Out]out HRESULT[] ppErrors);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LookupItemIDs(
            [MarshalAs(UnmanagedType.LPWStr), In] string szItemID, 
            [In] int dwCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), In] int[] pdwPropertyIDs,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1), Out]out string[] ppszNewItemIDs,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Struct, SizeParamIndex = 1), Out]out HRESULT[] ppErrors);
    }
}