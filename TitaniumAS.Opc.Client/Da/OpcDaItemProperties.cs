using System;
using System.Collections.Generic;
using System.Linq;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA item properties of single browsed element.
    /// </summary>
    public class OpcDaItemProperties
    {
        /// <summary>
        /// Gets or sets the error identifier.
        /// S_OK - the requested properties were returned for the item.
        /// S_FALSE - one or more requested properties were invalid for the item.
        /// OPC_E_INVALIDITEMID - the item name does not conform to the server's syntax.
        /// OPC_E_UNKNOWNITEMID - the item is not known in the server address space.
        /// </summary>
        /// <value>
        /// The error identifier.
        /// </value>
        public HRESULT ErrorId { get; set; }

        /// <summary>
        /// Gets or sets an array of properties.
        /// </summary>
        /// <value>
        /// The array of properties.
        /// </value>
        public OpcDaItemProperty[] Properties { get; set; }

        /// <summary>
        /// Creates the empty item properties.
        /// </summary>
        /// <returns>The empty item properties.</returns>
        public static OpcDaItemProperties CreateEmpty()
        {
            return new OpcDaItemProperties {Properties = new OpcDaItemProperty[0]};
        }

        /// <summary>
        /// Filters properties in this instance by specified property identifiers.
        /// </summary>
        /// <param name="propertyIds">The property identifiers.</param>
        /// <exception cref="System.ArgumentNullException">propertyIds</exception>
        public void IntersectProperties(IEnumerable<int> propertyIds)
        {
            if (propertyIds == null) throw new ArgumentNullException("propertyIds");

            ILookup<int, int> requiredIds = propertyIds.ToLookup(t => t);

            Properties = Properties.Where(p => requiredIds.Contains(p.PropertyId)).ToArray();
        }
    }
}