using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents arguments of OPC DA item values changed event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OpcDaItemValuesChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaItemValuesChangedEventArgs"/> class.
        /// </summary>
        /// <param name="values">The changed item values.</param>
        public OpcDaItemValuesChangedEventArgs(OpcDaItemValue[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets or sets the changed item values.
        /// </summary>
        /// <value>
        /// The changed item values.
        /// </value>
        public OpcDaItemValue[] Values { get; set; }
    }
}