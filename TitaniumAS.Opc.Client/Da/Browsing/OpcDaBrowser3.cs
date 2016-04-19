using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using TitaniumAS.Opc.Client.Da.Wrappers;

namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Represents an OPC DA 3.0 browser.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.Browsing.IOpcDaBrowser" />
    public class OpcDaBrowser3 : IOpcDaBrowser
    {
        private static readonly ILog Log = LogManager.GetLogger<OpcDaBrowser3>();
        private OpcDaServer _opcDaServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaBrowser3"/> class.
        /// </summary>
        /// <param name="opcDaServer">The OPC DA server for browsing.</param>
        public OpcDaBrowser3(OpcDaServer opcDaServer = null)
        {
            OpcDaServer = opcDaServer;
        }
       
        protected OpcBrowse OpcBrowse { get; set; }

        /// <summary>
        /// Gets or sets the OPC DA server for browsing.
        /// </summary>
        /// <value>
        /// The OPC DA server.
        /// </value>
        public OpcDaServer OpcDaServer
        {
            get { return _opcDaServer; }
            set
            {
                _opcDaServer = value;
                if (_opcDaServer != null)
                {
                    OpcBrowse = _opcDaServer.As<OpcBrowse>();
                }
                else
                {
                    OpcBrowse = null;
                }
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
        /// <exception cref="System.InvalidOperationException">Browser isn't attached to com object.</exception>
        public OpcDaBrowseElement[] GetElements(string parentItemId, OpcDaElementFilter filter = null,
            OpcDaPropertiesQuery propertiesQuery = null)
        {
            if (OpcBrowse == null)
                throw new InvalidOperationException("Browser isn't attached to com object.");

            if (parentItemId == null)
                parentItemId = string.Empty;

            if (filter == null)
                filter = new OpcDaElementFilter();

            bool returnAllProperties;
            bool returnPropertyValues;
            IList<int> propertyIds;
            if (propertiesQuery != null)
            {
                returnAllProperties = propertiesQuery.AllProperties;
                returnPropertyValues = propertiesQuery.ReturnValues;
                propertyIds = propertiesQuery.PropertyIds;
            }
            else
            {
                returnAllProperties = false;
                returnPropertyValues = false;
                propertyIds = new int[0];
            }

            var opcBrowseElements = OpcBrowse.Browse(parentItemId, filter.ElementType, filter.Name,
                filter.VendorSpecific,
                returnAllProperties, returnPropertyValues, propertyIds);

            if (OpcConfiguration.EnableQuirks && opcBrowseElements.Length > 0 && returnAllProperties &&
                NoPropertiesReturned(opcBrowseElements))
            {
                // bad server implementation of Browse. For example some versions of Matrikon simulation server
                // all properties was requested but returned nothing.
                // Requery properties with GetProperties
                var properties = GetProperties(opcBrowseElements.Select(e => e.ItemId).ToArray(), propertiesQuery);
                for (var i = 0; i < opcBrowseElements.Length; i++)
                {
                    opcBrowseElements[i].ItemProperties = properties[i];
                }
            }

            return opcBrowseElements;
        }

        /// <summary>
        /// Gets properties of items by specified item identifiers.
        /// </summary>
        /// <param name="itemIds">The item identifiers.</param>
        /// <param name="propertiesQuery">The properties query.</param>
        /// <returns>
        /// Array of properties of items.
        /// </returns>
        public OpcDaItemProperties[] GetProperties(IList<string> itemIds, OpcDaPropertiesQuery propertiesQuery = null)
        {
            if (propertiesQuery == null)
                propertiesQuery = new OpcDaPropertiesQuery();

            var properties = OpcBrowse.GetProperties(itemIds, propertiesQuery.ReturnValues, propertiesQuery.PropertyIds);
            if (OpcConfiguration.EnableQuirks && properties.Length > 0 && propertiesQuery.AllProperties &&
                NoPropertiesReturned(properties))
            {
                // TODO: fallback to IOPCItemProperties
            }
            return properties;
        }

        private static bool NoPropertiesReturned(OpcDaBrowseElement[] opcBrowseElements)
        {
            return NoPropertiesReturned(opcBrowseElements.Select(e => e.ItemProperties));
        }

        private static bool NoPropertiesReturned(IEnumerable<OpcDaItemProperties> properties)
        {
            var propertiesWithoutErrors = properties.Where(p => p.ErrorId.Succeeded);
            if (!propertiesWithoutErrors.Any())
            {
                return false; // all properties with errors
            }
            return !propertiesWithoutErrors.Any(p => p.Properties.Length > 0);
        }
    }
}