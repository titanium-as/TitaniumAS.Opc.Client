using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Common;


namespace TitaniumAS.Opc.Client.Common.Internal
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    internal struct CATEGORYINFO
    {
        public Guid catid;
        public uint lcid;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szDescription;
    }

    [ComImport, Guid("0002E011-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
     ComVisible(false)]
    internal interface IEnumCATEGORYINFO
    {
        int Next(int celt, [Out, In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] CATEGORYINFO[] rgelt);
        void Skip(int celt);
        void Reset();

        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumCATEGORYINFO Clone();
    }


    /// <summary>
    /// provides methods for obtaining information about the categories 
    /// implemented or required by a certain class and provides information 
    /// about the categories registered on a given machine
    /// </summary>
    [ComImport, Guid("0002E013-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
     ComVisible(false)]
    internal interface ICatInformation
    {
        /// <summary>
        /// Returns an enumerator for the component categories registered on the system.
        /// </summary>
        /// <param name="lcid">Identifies the requested locale for any return szDescription of the enumerated CATEGORYINFOs.</param>
        /// <returns>can be used to enumerate the CATIDs and localized description strings of the component categories registered with the system.</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumCATEGORYINFO EnumCategories(uint lcid);

        /// <summary>
        /// Retrieves the localized description string for a specific category ID.
        /// </summary>
        /// <param name="rcatid">Identifies the category for which the description string is to be returned.</param>
        /// <param name="lcid">Specifies the locale in which the resulting string is returned.</param>
        /// <returns>The description</returns>
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetCategoryDesc([In] ref Guid rcatid, uint lcid);

        /// <summary>
        /// Returns an enumerator over the classes that implement one or more 
        /// of rgcatidImpl. If a class requires a category not listed in 
        /// rgcatidReq, it is not included in the enumeration.
        /// </summary>
        /// <param name="cImplemented">
        /// The number of category IDs in the rgcatidImpl array. This value 
        /// cannot be zero. If this value is -1, classes are included in the 
        /// enumeration, regardless of the categories they implement.
        /// </param>
        /// <param name="rgcatidImpl">An array of category identifiers.</param>
        /// <param name="cRequired">The number of category IDs in the rgcatidReq 
        /// array. This value can be zero. If this value is -1, classes are 
        /// included in the enumeration, regardless of the categories they require. 
        /// </param>
        /// <param name="rgcatidReq">An array of category identifiers.</param>
        /// <returns>An IEnumCLSID interface that can be used to enumerate the 
        /// CLSIDs of the classes that implement category rcatid.</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumGUID RemoteEnumClassesOfCategories(int cImplemented,
            [In, MarshalAs(UnmanagedType.LPArray)] Guid[] rgcatidImpl, int cRequired,
            [In, MarshalAs(UnmanagedType.LPArray)] Guid[] rgcatidReq);

        /// <summary>
        /// Determines if a class implements one or more categories. If the 
        /// class requires a category not listed in rgcatidReq, it is not 
        /// included in the enumeration.
        /// </summary>
        /// <param name="rclsid">The class ID of the relevent class to query.</param>
        /// <param name="cImplemented">The number of category IDs in the rgcatidImpl 
        /// array. This value cannot be zero. If this value is -1, the implemented 
        /// categories are not tested. </param>
        /// <param name="rgcatidImpl">An array of category identifiers.</param>
        /// <param name="cRequired">The number of category IDs in the rgcatidReq 
        /// array. This value can be zero. If this value is -1, the required 
        /// categories are not tested. 
        /// </param> 
        /// <param name="rgcatidReq">An array of category identifiers.</param>
        /// <returns>
        /// S_OK rclsid is of category rcatid. 
        /// S_FALSE rclsid is not of category rcatid. 
        /// </returns>
        int RemoteIsClassOfCategories([In] ref Guid rclsid,
            int cImplemented, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] rgcatidImpl,
            int cRequired, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] rgcatidReq);

        /// <summary>
        /// Returns an enumerator for the CATIDs implemented by the specified class.
        /// </summary>
        /// <param name="rclsid">Specifies the class ID.</param>
        /// <returns>an IEnumCATID interface that can be used to enumerate the 
        /// CATIDs that are implemented by rclsid.</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumGUID EnumImplCategoriesOfClass([In] ref Guid rclsid);

        /// <summary>
        /// Returns an enumerator for the CATIDs required by the specified class.
        /// </summary>
        /// <param name="rclsid">Specifies the class ID.</param>
        /// <returns>an IEnumCATID interface that can be used to enumerate the 
        /// CATIDs that are required by rclsid.</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumGUID EnumReqCategoriesOfClass([In] ref Guid rclsid);
    };

    [
        ComImport,
        Guid("0002E005-0000-0000-C000-000000000046")
    ]
    internal class StdComponentCategoriesMgr
    {
    };
}