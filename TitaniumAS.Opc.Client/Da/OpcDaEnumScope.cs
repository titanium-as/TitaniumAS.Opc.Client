using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents enumerators for OPC DA groups.
    /// </summary>
    public enum OpcDaEnumScope
    {
        /// <summary>
        /// Enumerate private connections.
        /// </summary>
        PrivateConnections = OPCENUMSCOPE.OPC_ENUM_PRIVATE_CONNECTIONS,

        /// <summary>
        /// Enumerate public connections.
        /// </summary>
        PublicConnections = OPCENUMSCOPE.OPC_ENUM_PUBLIC_CONNECTIONS,

        /// <summary>
        /// Enumerate all connections.
        /// </summary>
        AllConnections = OPCENUMSCOPE.OPC_ENUM_ALL_CONNECTIONS,

        /// <summary>
        /// Enumerate all private groups created by the client.
        /// </summary>
        Private = OPCENUMSCOPE.OPC_ENUM_PRIVATE,

        /// <summary>
        /// Enumerate all public groups available in the server.
        /// </summary>
        Public = OPCENUMSCOPE.OPC_ENUM_PUBLIC,

        /// <summary>
        /// Enumerate all private groups.
        /// </summary>
        All = OPCENUMSCOPE.OPC_ENUM_ALL
    }
}