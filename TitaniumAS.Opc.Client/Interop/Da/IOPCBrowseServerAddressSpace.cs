using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Common;

namespace TitaniumAS.Opc.Client.Interop.Da
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("39c13a4f-011e-11d0-9675-0020afd8adb3")]
    [ComImport]
    internal interface IOPCBrowseServerAddressSpace
    {
        void QueryOrganization([MarshalAs(UnmanagedType.U4)] out OPCNAMESPACETYPE pNameSpaceType);

        void ChangeBrowsePosition([MarshalAs(UnmanagedType.U4), In] OPCBROWSEDIRECTION dwBrowseDirection,
            [MarshalAs(UnmanagedType.LPWStr), In] string szName);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int BrowseOPCItemIDs([MarshalAs(UnmanagedType.U4), In] OPCBROWSETYPE dwBrowseFilterType,
            [MarshalAs(UnmanagedType.LPWStr), In] string szFilterCriteria,
            [MarshalAs(UnmanagedType.U2), In] short vtDataTypeFilter,
            [MarshalAs(UnmanagedType.U4), In] OPCACCESSRIGHTS dwAccessRightsFilter,
            [MarshalAs(UnmanagedType.Interface)] out IEnumString ppIEnumString);

        void GetItemID([MarshalAs(UnmanagedType.LPWStr), In] string szItemDataID,
            [MarshalAs(UnmanagedType.LPWStr)] out string szItemID);

        [MethodImpl(MethodImplOptions.PreserveSig)]
        int BrowseAccessPaths([MarshalAs(UnmanagedType.LPWStr), In] string szItemID,
            [MarshalAs(UnmanagedType.Interface)] out IEnumString ppIEnumString);
    }
}