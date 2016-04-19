using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [Guid("39227004-A18F-4B57-8B0A-5235670F4468")]
    [InterfaceType(1)]
    [ComConversionLoss]
    [ComImport]
    internal interface IOPCBrowse
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetProperties(
            [In] int dwItemCount,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 0), In] string[] pszItemIDs,
            [MarshalAs(UnmanagedType.Bool),In] bool bReturnPropertyValues, 
            [In] int dwPropertyCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3), In] int[] pdwPropertyIDs, 
            out IntPtr ppItemProperties);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Browse(
            [MarshalAs(UnmanagedType.LPWStr), In] string szItemID, 
            [In, Out] ref IntPtr pszContinuationPoint,
            [In] int dwMaxElementsReturned, 
            [In] OPCBROWSEFILTER dwBrowseFilter,
            [MarshalAs(UnmanagedType.LPWStr), In] string szElementNameFilter,
            [MarshalAs(UnmanagedType.LPWStr), In] string szVendorFilter, 
            [MarshalAs(UnmanagedType.Bool), In] bool bReturnAllProperties,
            [MarshalAs(UnmanagedType.Bool), In] bool bReturnPropertyValues, 
            [In] int dwPropertyCount,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 8), In] int[] pdwPropertyIDs, 
            out int pbMoreElements,
            out int pdwCount, 
            out IntPtr ppBrowseElements);
    }
}