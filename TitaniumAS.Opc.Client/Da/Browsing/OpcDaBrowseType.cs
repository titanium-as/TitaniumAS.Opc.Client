using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Represents OPC DA browse type.
    /// </summary>
    public enum OpcDaBrowseType
    {
        /// <summary>
        /// Obtain only items which have children.
        /// </summary>
        Branch = OPCBROWSETYPE.OPC_BRANCH,

        /// <summary>
        /// Obtain only items which do not have children.
        /// </summary>
        Leaf = OPCBROWSETYPE.OPC_LEAF,

        /// <summary>
        /// Obtain everything at this level and below without branches.
        /// </summary>
        Flat = OPCBROWSETYPE.OPC_FLAT
    }
}