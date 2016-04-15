using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Wrappers;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da.Browsing.Internal
{
    internal static class OpcItemPropertiesExtensions
    {
        public static OpcDaItemProperty[] GetItemProperties(this OpcItemProperties opcItemProperties, string itemId,
            IList<int> propertyIds)
        {
            HRESULT[] ppErrors;
            var ppvData = opcItemProperties.GetItemProperties(itemId, propertyIds.ToArray(), out ppErrors);


            var properties = new OpcDaItemProperty[ppvData.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                var property = new OpcDaItemProperty
                {
                    PropertyId = propertyIds[i],
                    Value = ppvData[i],
                    ErrorId = ppErrors[i]
                };
                properties[i] = property;
            }

            return properties;
        }

        public static OpcDaItemProperties GetItemProperties(this OpcItemProperties opcItemProperties, string itemId,
            OpcDaItemProperties properties)
        {
            opcItemProperties.GetItemProperties(itemId, properties.Properties);
            return properties;
        }

        public static IList<OpcDaItemProperty> GetItemProperties(this OpcItemProperties opcItemProperties, string itemId,
            IList<OpcDaItemProperty> properties)
        {
            HRESULT[] ppErrors;
            var propertyIds = GetPropertyIds(properties);
            var ppvData = opcItemProperties.GetItemProperties(itemId, propertyIds, out ppErrors);


            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                property.Value = ppvData[i];
                property.ErrorId = ppErrors[i];
                properties[i] = property;
            }

            return properties;
        }

        private static int[] GetPropertyIds(IList<OpcDaItemProperty> properties)
        {
            var propertyIds = new int[properties.Count];
            for (var i = 0; i < propertyIds.Length; i++)
            {
                propertyIds[i] = properties[i].PropertyId;
            }
            return propertyIds;
        }

        public static OpcDaItemProperties QueryAvailableProperties(this OpcItemProperties opcItemProperties,
            string itemId)
        {
            string[] ppDescriptions;
            VarEnum[] ppvtDataTypes;
            var pdwPropertyIDs = opcItemProperties.QueryAvailableProperties(itemId, out ppDescriptions,
                out ppvtDataTypes);

            var props = new OpcDaItemProperties {Properties = new OpcDaItemProperty[pdwPropertyIDs.Length]};
            for (var i = 0; i < pdwPropertyIDs.Length; i++)
            {
                var property = new OpcDaItemProperty
                {
                    PropertyId = pdwPropertyIDs[i],
                    Description = ppDescriptions[i],
                    DataType = TypeConverter.FromVarEnum(ppvtDataTypes[i])
                };
                props.Properties[i] = property;
            }
            return props;
        }

        public static OpcDaItemProperty[] LookupItemIDs(this OpcItemProperties opcItemProperties, string itemId,
            IList<int> propertyIds)
        {
            HRESULT[] ppErrors;
            var ppszNewItemIDs = opcItemProperties.LookupItemIDs(itemId, propertyIds.ToArray(), out ppErrors);

            var properties = new OpcDaItemProperty[propertyIds.Count];
            for (var i = 0; i < propertyIds.Count; i++)
            {
                var property = new OpcDaItemProperty
                {
                    PropertyId = propertyIds[i],
                    ItemId = ppszNewItemIDs[i],
                    ErrorId = ppErrors[i]
                };
                properties[i] = property;
            }

            return properties;
        }

        public static IList<OpcDaItemProperty> LookupItemIDs(this OpcItemProperties opcItemProperties, string itemId,
            IList<OpcDaItemProperty> properties)
        {
            var propertyIds = GetPropertyIds(properties);
            HRESULT[] ppErrors;

            var ppszNewItemIDs = opcItemProperties.LookupItemIDs(itemId, propertyIds, out ppErrors);

            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                property.ItemId = ppszNewItemIDs[i];
                property.ErrorId = ppErrors[i];
            }

            return properties;
        }

        public static OpcDaItemProperties LookupItemIDs(this OpcItemProperties opcItemProperties, string itemId,
            OpcDaItemProperties properties)
        {
            opcItemProperties.LookupItemIDs(itemId, properties.Properties);
            return properties;
        }
    }
}