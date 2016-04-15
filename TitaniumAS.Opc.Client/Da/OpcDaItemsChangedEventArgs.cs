using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents arguments of OPC DA items changed event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OpcDaItemsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaItemsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="added">The added items.</param>
        /// <param name="removed">The removed items.</param>
        /// <param name="changed">The changed items.</param>
        public OpcDaItemsChangedEventArgs(OpcDaItem[] added, OpcDaItem[] removed, OpcDaItem[] changed)
        {
            Added = added;
            Removed = removed;
            Changed = changed;
        }

        /// <summary>
        /// Gets the added items.
        /// </summary>
        /// <value>
        /// The added items.
        /// </value>
        public OpcDaItem[] Added { get; private set; }

        /// <summary>
        /// Gets the removed items.
        /// </summary>
        /// <value>
        /// The removed items.
        /// </value>
        public OpcDaItem[] Removed { get; private set; }

        /// <summary>
        /// Gets the changed items.
        /// </summary>
        /// <value>
        /// The changed items.
        /// </value>
        public OpcDaItem[] Changed { get; private set; }
    }
}