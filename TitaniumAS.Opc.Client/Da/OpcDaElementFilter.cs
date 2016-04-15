using System;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents filtering context for getting OPC elements unsing OPC browser.
    /// </summary>
    public class OpcDaElementFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaElementFilter"/> class.
        /// </summary>
        public OpcDaElementFilter()
        {
            Name = String.Empty;
            DataType = null;
            AccessRights = OpcDaAccessRights.Ignore;
            VendorSpecific = String.Empty;
            ElementType = OpcDaBrowseFilter.All;
        }

        /// <summary>
        /// For OPC DA 2.05: gets or sets server specific filter criteria.
        /// For OPC DA 3.0: gets or sets an element name filter (a wildcard that conforms to the Visual Basic LIKE operator. It uses to filter Element names).
        /// There is no filter if null.
        /// </summary>
        /// <value>
        /// The filter criteria or the element name filter.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// For OPC DA 2.05 only: gets or sets a filter by data type. Filter the result by avalaible datatypes.
        /// </summary>
        /// <value>
        /// The data type.
        /// </value>
        public Type DataType { get; set; }

        /// <summary>
        ///  For OPC DA 2.05 only: gets or sets a filter by access rights.
        /// </summary>
        /// <value>
        /// The access rights.
        /// </value>
        public OpcDaAccessRights AccessRights { get; set; }

        /// <summary>
        /// For OPC DA 3.0 only: gets or sets the vendor specific filter. There is no filter if null.
        /// </summary>
        /// <value>
        /// The vendor specific filter.
        /// </value>
        public string VendorSpecific { get; set; }

        /// <summary>
        /// For OPC DA 2.05 and OPC DA 3.0. Gets or sets the filter flags determine which types of browse elements will be returned.
        /// </summary>
        /// <value>
        /// The type of the element.
        /// </value>
        public OpcDaBrowseFilter ElementType { get; set; }
    }
}
