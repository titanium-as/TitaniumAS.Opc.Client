using System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    /// Represents arguments of OPC server shutdown event.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class OpcShutdownEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcShutdownEventArgs" /> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="error">The error code.</param>
        public OpcShutdownEventArgs(string reason, HRESULT error)
        {
            Reason = reason;
            Error = error;
        }

        /// <summary>
        /// Gets an optional text provided by the OPC server indicating the reason for the shutdown.
        /// </summary>
        /// <value>
        /// The reason of shutdown event.
        /// </value>
        public string Reason { get; private set; }

        /// <summary>
        /// Gets the error code (HRESULT).
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public HRESULT Error { get; private set; }
    }
}