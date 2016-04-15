using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents OPC DA namespace type enumeration.
    /// </summary>
    public enum OpcDaNamespaceType
    {
        /// <summary>
        /// The hierarchial namespace.
        /// </summary>
        Hierarchial = OPCNAMESPACETYPE.OPC_NS_HIERARCHIAL,

        /// <summary>
        /// The flat namespace.
        /// </summary>
        Flat = OPCNAMESPACETYPE.OPC_NS_FLAT
    }
}