using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents arguments of an OPC DA server connection state changed event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OpcDaServerConnectionStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaServerConnectionStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="isConnected">if set to <c>true</c> the server is connected.</param>
        public OpcDaServerConnectionStateChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the server currently connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; set; }
    }
}