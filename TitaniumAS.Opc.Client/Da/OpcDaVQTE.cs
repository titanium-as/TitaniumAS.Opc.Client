using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a combination of value, quality, timestamp and HRESULT of an operation for specific item.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.OpcDaVQT" />
    public class OpcDaVQTE : OpcDaVQT
    {
        /// <summary>
        /// Gets or sets the HRESULT of an operation.
        /// </summary>
        /// <value>
        /// The HRESULT.
        /// </value>
        public HRESULT Error { get; set; }
    }
}