using System;
using System.Collections.Generic;
using Common.Logging;

namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Represents a browser which automatically browse an OPC DA server using different protocol versions in the following order 3.0, 2.05, 1.0.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.Browsing.IOpcDaBrowser" />
    public class OpcDaBrowserAuto : IOpcDaBrowser
    {
        private static readonly ILog Log = LogManager.GetLogger<OpcDaBrowserAuto>();
        private readonly OpcDaBrowser1 _browser1 = new OpcDaBrowser1();
        private readonly OpcDaBrowser2 _browser2 = new OpcDaBrowser2();
        private readonly OpcDaBrowser3 _browser3 = new OpcDaBrowser3();
        private OpcDaServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaBrowserAuto"/> class.
        /// </summary>
        /// <param name="server">The OPC DA server for browsing.</param>
        public OpcDaBrowserAuto(OpcDaServer server = null)
        {
            OpcDaServer = server;
        }

        /// <summary>
        /// Gets or sets the OPC DA server for browsing.
        /// </summary>
        /// <value>
        /// The OPC DA server.
        /// </value>
        public OpcDaServer OpcDaServer
        {
            get { return _server; }
            set
            {
                _server = value;
                _browser1.OpcDaServer = value;
                _browser2.OpcDaServer = value;
                _browser3.OpcDaServer = value;
            }
        }

        /// <summary>
        /// Browse the server using specified criteria.
        /// </summary>
        /// <param name="parentItemId">Indicates the item identifier from which browsing will begin. If the root branch is to be browsed then a null should be passed.</param>
        /// <param name="filter">The filtering context.</param>
        /// <param name="propertiesQuery">The properties query.</param>
        /// <returns>
        /// Array of browsed elements.
        /// </returns>
        /// <exception cref="System.AggregateException"></exception>
        public OpcDaBrowseElement[] GetElements(string parentItemId, OpcDaElementFilter filter = null,
            OpcDaPropertiesQuery propertiesQuery = null)
        {
            try
            {
                return _browser3.GetElements(parentItemId, filter, propertiesQuery);
            }
            catch (Exception ex1)
            {
                Log.Warn("Failed to browse address space using IOPCBrowse (OPC DA 3.x).", ex1);
                try
                {
                    return _browser2.GetElements(parentItemId, filter, propertiesQuery);
                }
                catch (Exception ex2)
                {
                    Log.Warn("Failed to browse address space using IOPCBrowseServerAddressSpace (OPC DA 2.x).", ex2);
                    try
                    {
                        return _browser1.GetElements(parentItemId, filter, propertiesQuery);
                    }
                    catch (Exception ex3)
                    {
                        Log.Warn("Failed to browse address space using IOPCBrowseServerAddressSpace (OPC DA 1.x).", ex2);
                        throw new AggregateException(ex3, ex2, ex1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets properties of items by specified item identifiers.
        /// </summary>
        /// <param name="itemIds">The item identifiers.</param>
        /// <param name="propertiesQuery">The properties query.</param>
        /// <returns>
        /// Array of properties of items.
        /// </returns>
        /// <exception cref="System.AggregateException"></exception>
        public OpcDaItemProperties[] GetProperties(IList<string> itemIds, OpcDaPropertiesQuery propertiesQuery = null)
        {
            try
            {
                return _browser3.GetProperties(itemIds, propertiesQuery);
            }
            catch (Exception ex1)
            {
                Log.Warn("Failed to get properties using IOPCBrowse (OPC DA 3.x).", ex1);
                try
                {
                    return _browser2.GetProperties(itemIds, propertiesQuery);
                }
                catch (Exception ex2)
                {
                    Log.Warn("Failed to get properties using IOPCItemProperties (OPC DA 2.x).", ex2);
                    try
                    {
                        return _browser1.GetProperties(itemIds, propertiesQuery);
                    }
                    catch (Exception ex3)
                    {
                        Log.Warn("Failed to get properties using IOPCItemProperties (OPC DA 1.x).", ex2);
                        throw new AggregateException(ex3, ex2, ex1);
                    }
                }
            }
        }
    }
}