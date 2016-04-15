using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a result of operation with an item.
    /// </summary>
    public class OpcDaItemResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaItemResult"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="error">The HRESULT of the operation.</param>
        public OpcDaItemResult(OpcDaItem item, HRESULT error)
        {
            Item = item;
            Error = error;
        }

        /// <summary>
        /// Gets or sets the HRESULT of the operation.
        /// </summary>
        /// <value>
        /// The HRESULT of the operation.
        /// </value>
        public HRESULT Error { get; set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public OpcDaItem Item { get; set; }
    }
}