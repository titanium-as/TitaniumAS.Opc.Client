using System;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA item property of single browsed element.
    /// </summary>
    public class OpcDaItemProperty
    {
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{1} {3} (ItemId: {2}, DataType {0}, Value: {4}, ErrorId: {5})", DataType, PropertyId, ItemId, Description, Value, ErrorId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaItemProperty"/> class.
        /// </summary>
        public OpcDaItemProperty()
        {
        }

        /// <summary>
        /// Gets or sets the canonical data type of the item property.
        /// </summary>
        /// <value>
        /// The data type of the item property.
        /// </value>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the property identifier of the item property.
        /// </summary>
        /// <value>
        /// The property identifier.
        /// </value>
        public int PropertyId { get; set; }


        /// <summary>
        /// Gets or sets a fully qualified item identifier that can be used to access the property. If null is returned the property cannot be accessed via an item identifier.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets a non-localized text description of the property.
        /// </summary>
        /// <value>
        /// The description of the property.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current value of the property. If values were not requested it is of type VT_EMPTY.
        /// </summary>
        /// <value>
        /// The current value of the property.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the error identifier. If FAILED only PropertyId will contain a known valid value. All other properties may contain invalid data.
        /// S_OK - the corresponding PropertyId is valid and the structure contains accurate information.
        /// OPC_E_INVALID_PID - the passed PropertyId is not defined for this item.
        /// </summary>
        /// <value>
        /// The error identifier.
        /// </value>
        public HRESULT ErrorId { get; set; }
    }
}