using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Indicates the browse direction.
    /// </summary>
    public enum OpcDaBrowseDirection
    {
        /// <summary>
        /// Browse up.
        /// </summary>
        Up = OPCBROWSEDIRECTION.OPC_BROWSE_UP,

        /// <summary>
        /// Browse down.
        /// </summary>
        Down = OPCBROWSEDIRECTION.OPC_BROWSE_DOWN,

        /// <summary>
        /// Browse to.
        /// </summary>
        To = OPCBROWSEDIRECTION.OPC_BROWSE_TO
    }
}