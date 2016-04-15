using System.Collections.Generic;
using System.Linq;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA properties query.
    /// </summary>
    public class OpcDaPropertiesQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaPropertiesQuery"/> class.
        /// </summary>
        /// <param name="returnValues">if set to <c>true</c> the query will return values.</param>
        /// <param name="propertIds">The property identifiers.</param>
        public OpcDaPropertiesQuery(bool returnValues, params int[] propertIds)
        {
            ReturnValues = returnValues;
            PropertyIds = propertIds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaPropertiesQuery"/> class.
        /// </summary>
        /// <param name="returnValues">if set to <c>true</c> the query will return values.</param>
        /// <param name="propertIds">The property identifiers.</param>
        public OpcDaPropertiesQuery(bool returnValues, params OpcDaItemPropertyIds[] propertIds)
        {
            ReturnValues = returnValues;
            PropertyIds = propertIds.Cast<int>().ToArray();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaPropertiesQuery"/> class.
        /// </summary>
        /// <param name="returnValues">if set to <c>true</c> the query will return values.</param>
        public OpcDaPropertiesQuery(bool returnValues = false)
        {
            PropertyIds = null; // all propeties
            ReturnValues = returnValues;
        }

        /// <summary>
        /// Gets or sets identifiers for requested properties.
        /// </summary>
        /// <value>
        /// The property identifiers.
        /// </value>
        public IList<int> PropertyIds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a list of current data values should be returned for given property identifiers.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the query will return values; otherwise, <c>false</c>.
        /// </value>
        public bool ReturnValues { get; set; }

        /// <summary>
        /// Gets a value indicating that all properties will be selected for a query.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all properties will be selected for a query; otherwise, <c>false</c>.
        /// </value>
        public bool AllProperties { get { return PropertyIds == null; } }
    }
}