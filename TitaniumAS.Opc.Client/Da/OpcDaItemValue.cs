using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Common.Logging;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents an OPC DA item value with value, quality, timestamp and HRESULT of an operation.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.OpcDaVQTE" />
    public class OpcDaItemValue : OpcDaVQTE
    {
        private static readonly ILog Log = LogManager.GetLogger<OpcDaItemValue>();

        /// <summary>
        /// Gets or sets the item to which the value object belongs to.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public OpcDaItem Item { get; set; }

        internal static OpcDaItemValue[] Create(IList<OpcDaItem> items, object[] ppvValues, OpcDaQuality[] ppwQualities,
            DateTimeOffset[] ppftTimeStamps, HRESULT[] ppErrors)
        {
            try
            {
                var result = new OpcDaItemValue[items.Count];
                for (var i = 0; i < ppvValues.Length; i++)
                {
                    var tagValue = new OpcDaItemValue
                    {
                        Value = ppvValues[i],
                        Quality = ppwQualities[i],
                        Timestamp = ppftTimeStamps[i],
                        Error = ppErrors[i],
                        Item = items[i]
                    };
                    result[i] = tagValue;
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Cannot create OpcDaItemValue object.", ex);
                return new OpcDaItemValue[0];
            }
        }

        internal static OpcDaItemValue[] Create(OpcDaGroup opcDaGroup, OPCITEMSTATE[] ppItemValues,
            HRESULT[] ppErrors)
        {
            try
            {
                var result = new OpcDaItemValue[ppItemValues.Length];
                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = new OpcDaItemValue
                    {
                        Timestamp = FileTimeConverter.FromFileTime(ppItemValues[i].ftTimeStamp),
                        Value = ppItemValues[i].vDataValue,
                        Quality = ppItemValues[i].wQuality,
                        Error = ppErrors[i],
                        Item = opcDaGroup.GetItem(ppItemValues[i].hClient)
                    };
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("Cannot create OpcDaItemValue object.", ex);
                return new OpcDaItemValue[0];
            }
        }

        internal static OpcDaItemValue[] Create(OpcDaGroup opcDaGroup, int dwCount, int[] phClientItems,
            object[] pvValues,
            short[] pwQualities, FILETIME[] pftTimeStamps, HRESULT[] pErrors)
        {
            try
            {
                var values = new OpcDaItemValue[dwCount];
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = new OpcDaItemValue
                    {
                        Value = pvValues[i],
                        Item = opcDaGroup.GetItem(phClientItems[i]),
                        Quality = pwQualities[i],
                        Timestamp = FileTimeConverter.FromFileTime(pftTimeStamps[i]),
                        Error = pErrors[i]
                    };
                }
                return values;
            }
            catch (Exception ex)
            {
                Log.Error("Cannot create OpcDaItemValue object.", ex);
                return new OpcDaItemValue[0];
            }
        }
    }
}