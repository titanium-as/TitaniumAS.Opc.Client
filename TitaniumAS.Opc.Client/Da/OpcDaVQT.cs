using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a combination of value, quality and timestamp for specific item.
    /// </summary>
    public class OpcDaVQT
    {
        /// <summary>
        /// Gets or sets the value object of specific item. If it equals to VT_EMPTY there is no value to write.
        /// </summary>
        /// <value>
        /// The value object.
        /// </value>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the quality of specific item.
        /// </summary>
        /// <value>
        /// The quality.
        /// </value>
        public OpcDaQuality Quality { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for the item value.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTimeOffset Timestamp { get; set; }
    }
}