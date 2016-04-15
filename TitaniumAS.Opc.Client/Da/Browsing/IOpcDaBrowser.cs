using System.Collections.Generic;

namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Represents common OPC DA browser interface.
    /// </summary>
    public interface IOpcDaBrowser
    {
        /// <summary>
        /// Gets or sets the OPC DA server for browsing.
        /// </summary>
        /// <value>
        /// The OPC DA server.
        /// </value>
        OpcDaServer OpcDaServer { get; set; }

        /// <summary>
        /// Browse the server using specified criteria.
        /// </summary>
        /// <param name="parentItemId">Indicates the item identifier from which browsing will begin. If the root branch is to be browsed then a null should be passed.</param>
        /// <param name="filter">The filtering context.</param>
        /// <param name="propertiesQuery">The properties query.</param>
        /// <returns>Array of browsed elements.</returns>
        OpcDaBrowseElement[] GetElements(string parentItemId, OpcDaElementFilter filter = null,
            OpcDaPropertiesQuery propertiesQuery = null);

        /// <summary>
        /// Gets properties of items by specified item identifiers.
        /// </summary>
        /// <param name="itemIds">The item identifiers.</param>
        /// <param name="propertiesQuery">The properties query.</param>
        /// <returns>Array of properties of items.</returns>
        OpcDaItemProperties[] GetProperties(IList<string> itemIds, OpcDaPropertiesQuery propertiesQuery = null);
    }
}