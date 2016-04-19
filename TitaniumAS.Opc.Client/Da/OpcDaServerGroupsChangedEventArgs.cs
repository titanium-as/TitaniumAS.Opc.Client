using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents arguments of an OPC DA server groups changed event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OpcDaServerGroupsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaServerGroupsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="added">The added groups.</param>
        /// <param name="removed">The removed groups.</param>
        public OpcDaServerGroupsChangedEventArgs(OpcDaGroup added, OpcDaGroup removed)
        {
            Added = added;
            Removed = removed;
        }

        /// <summary>
        /// Gets the added groups.
        /// </summary>
        /// <value>
        /// The added groups.
        /// </value>
        public OpcDaGroup Added { get; private set; }

        /// <summary>
        /// Gets the removed groups.
        /// </summary>
        /// <value>
        /// The removed groups.
        /// </value>
        public OpcDaGroup Removed { get; private set; }
    }
}