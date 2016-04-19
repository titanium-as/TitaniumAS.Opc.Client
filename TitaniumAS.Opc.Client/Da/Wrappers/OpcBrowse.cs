using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Browsing;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da.Wrappers
{
    public class OpcBrowse : ComWrapper
    {
        private const int OPC_BROWSE_HASCHILDREN = 1;
        private const int OPC_BROWSE_ISITEM = 2;

        public OpcBrowse(object comObject, object userData) : base(userData)
        {
            if (comObject == null) throw new ArgumentNullException("comObject");
            ComObject = DoComCall(comObject, "IUnknown::QueryInterface<IOPCBrowser>",
                () => comObject.QueryInterface<IOPCBrowse>());
        }

        internal IOPCBrowse ComObject { get; set; }

        /// <summary>
        ///     Browses a single branch of the address space and returns zero or more OPCBROWSEELEMENT
        ///     structures.
        ///     It is assumed that the underlying server address space is hierarchical. A flat space will always be
        ///     presented to the client as not having children. A hierarchical space can be presented to the client as
        ///     either not having children or having children.
        ///     A hierarchical presentation of the server address space would behave much like a file system, where
        ///     the directories are the branches or paths, and the files represent the leaves or items. For example, a
        ///     server could present a control system by showing all the control networks, then all of the devices on a
        ///     selected network, and then all of the classes of data within a device, then all of the data items of that
        ///     class. A further breakdown into vendor specific ‘Units’ and ‘Lines’ might be appropriate for a
        ///     BATCH system.
        ///     The browse position is initially set to the ‘root’ of the address space. On subsequent calls, the client
        ///     may choose to browse from the continuation point.
        ///     This browse can also be filtered by a vendor specific filter string.
        /// </summary>
        /// <param name="itemId">
        ///     The name of the branch in the hierarchy to browse. If the root branch is
        ///     to be browsed then a NUL string is passed.
        /// </param>
        /// <param name="filter">
        ///     An enumeration {All, Branch, Item} specifying which subset of browse
        ///     elements to return. See the table in the comments section below to
        ///     determine which combination of bits in
        ///     in dwBrowseFilter.
        /// </param>
        /// <param name="elementNameFilter">
        ///     A wildcard string that conforms to the Visual Basic LIKE operator,
        ///     which will be used to filter Element names. A NUL String implies no
        ///     filter.
        /// </param>
        /// <param name="vendorFilter">
        ///     A server specific filter string. This is entirely free format and may be
        ///     entered by the user via an EDIT field. A pointer to a NUL string indicates
        ///     no filtering.
        /// </param>
        /// <param name="returnAllProperties">
        ///     Server must return all properties which are available for each of the
        ///     returned elements. If true, pdwPropertyIDs is ignored.
        /// </param>
        /// <param name="returnPropertyValues">Server must return the property values in addition to the property names. </param>
        /// <param name="propertyIds">
        ///     An array of Property IDs to be returned with each element. if
        ///     bReturnAllProperties is true, pdwPropertyIDs is ignored and all
        ///     properties are returned.
        /// </param>
        /// <returns></returns>
        public OpcDaBrowseElement[] Browse(string itemId = "", OpcDaBrowseFilter filter = OpcDaBrowseFilter.All,
            string elementNameFilter = "",
            string vendorFilter = "", bool returnAllProperties = false, bool returnPropertyValues = false,
            IList<int> propertyIds = null)
        {
            IntPtr continuationPoint = IntPtr.Zero;
            int dwMaxElementsReturned = OpcConfiguration.BatchSize;
            var dwBrowseFilter = (OPCBROWSEFILTER) filter;
            string szElementNameFilter = elementNameFilter ?? string.Empty;
            string szVendorFilter = vendorFilter ?? string.Empty;
            int[] pdwPropertyIDs = propertyIds != null ? propertyIds.ToArray() : new int[0];
            int dwPropertyCount = propertyIds == null ? 0 : pdwPropertyIDs.Length;

            var elements = new List<OpcDaBrowseElement>();
            try
            {
                int pbMoreElements = 0;
                do
                {
                    try
                    {
                        var browseElements = new IntPtr();
                        int pdwCount = 0;
                        DoComCall(ComObject,"IOpcBrowse::Browse", () => ComObject.Browse(itemId, ref continuationPoint, dwMaxElementsReturned, dwBrowseFilter,
                                szElementNameFilter, szVendorFilter, returnAllProperties, returnPropertyValues,
                                dwPropertyCount, pdwPropertyIDs, out pbMoreElements, out pdwCount, out browseElements), itemId, dwMaxElementsReturned, dwBrowseFilter,
                            szElementNameFilter, szVendorFilter, returnAllProperties, returnPropertyValues,
                            dwPropertyCount, pdwPropertyIDs);
                        ReadBrowseElementsAndDealocate(ref browseElements, pdwCount, elements);
                    }
                    catch (Exception ex)
                    {
                        break; // stop browsing.
                    }
                } while ((continuationPoint != IntPtr.Zero && pbMoreElements != 0));
            }
            finally
            {
                Marshal.FreeCoTaskMem(continuationPoint);
            }

            return elements.ToArray();
        }

        public OpcDaItemProperties[] GetProperties(IList<string> itemIds, bool returnPropertyValues,
            IList<int> propertyIds)
        {
            var properties = new OpcDaItemProperties[itemIds.Count];

            var itemProperties = new IntPtr();
            if (propertyIds == null)
            {
                propertyIds = new int[0];
            }
            string[] pszItemIDs = itemIds.ToArray();
            int[] pdwPropertyIDs = propertyIds.ToArray();
            DoComCall(ComObject, "IOpcBrowser::GetProperties", () =>
                ComObject.GetProperties(pszItemIDs.Length, pszItemIDs, returnPropertyValues, pdwPropertyIDs.Length,
                    pdwPropertyIDs, out itemProperties), pszItemIDs.Length,
                pszItemIDs, returnPropertyValues, pdwPropertyIDs.Length, pdwPropertyIDs);

            IntPtr current = itemProperties;
            for (int i = 0; i < itemIds.Count; i++)
            {
                var opcItemProperties = (OPCITEMPROPERTIES) Marshal.PtrToStructure(current, typeof (OPCITEMPROPERTIES));
                properties[i] = ReadItemProperties(ref opcItemProperties);
                Marshal.DestroyStructure(current, typeof (OPCITEMPROPERTIES));
                current += Marshal.SizeOf(typeof (OPCITEMPROPERTIES));
            }
            Marshal.FreeCoTaskMem(itemProperties);

            return properties;
        }

        private static void ReadBrowseElementsAndDealocate(ref IntPtr browseElements, int pdwCount,
            List<OpcDaBrowseElement> elements)
        {
            IntPtr current = browseElements;
            for (int i = 0; i < pdwCount; i++)
            {
                elements.Add(ReadBrowseElementAndDealocate(ref current));
                current += Marshal.SizeOf(typeof (OPCBROWSEELEMENT));
            }
            Marshal.FreeCoTaskMem(browseElements);
            browseElements = IntPtr.Zero;
        }

        private static OpcDaBrowseElement ReadBrowseElementAndDealocate(ref IntPtr browseElement)
        {
            var opcBrowseElement = (OPCBROWSEELEMENT) Marshal.PtrToStructure(browseElement, typeof (OPCBROWSEELEMENT));
            var result = new OpcDaBrowseElement
            {
                Name = opcBrowseElement.szName,
                ItemId = opcBrowseElement.szItemID,
                HasChildren = (opcBrowseElement.dwFlagValue & OPC_BROWSE_HASCHILDREN) != 0,
                IsItem = (opcBrowseElement.dwFlagValue & OPC_BROWSE_ISITEM) != 0,
                ItemProperties = ReadItemProperties(ref opcBrowseElement.ItemProperties)
            };
            Marshal.DestroyStructure(browseElement, typeof (OPCBROWSEELEMENT));
            return result;
        }

        private static OpcDaItemProperties ReadItemProperties(ref OPCITEMPROPERTIES itemProperties)
        {
            var result = new OpcDaItemProperties
            {
                ErrorId = itemProperties.hrErrorID,
                Properties = ReadItemProperties(ref itemProperties.pItemProperties, itemProperties.dwNumProperties)
            };
            return result;
        }

        private static OpcDaItemProperty[] ReadItemProperties(ref IntPtr pItemProperties, int dwNumProperties)
        {
            var result = new OpcDaItemProperty[dwNumProperties];
            IntPtr current = pItemProperties;
            for (int i = 0; i < dwNumProperties; i++)
            {
                var opcitemproperty = (OPCITEMPROPERTY) Marshal.PtrToStructure(current, typeof (OPCITEMPROPERTY));
                result[i] = new OpcDaItemProperty
                {
                    DataType = TypeConverter.FromVarEnum((VarEnum) opcitemproperty.vtDataType),
                    PropertyId = opcitemproperty.dwPropertyID,
                    ItemId = opcitemproperty.szItemID,
                    Description = opcitemproperty.szDescription,
                    Value = opcitemproperty.vValue,
                    ErrorId = opcitemproperty.hrErrorID
                };
                Marshal.DestroyStructure(current, typeof (OPCITEMPROPERTY));
                current += Marshal.SizeOf(typeof (OPCITEMPROPERTY));
            }
            Marshal.FreeCoTaskMem(pItemProperties);
            pItemProperties = IntPtr.Zero;
            return result;
        }
    }
}