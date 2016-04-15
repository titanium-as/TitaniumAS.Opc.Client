using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the type of Engineering Units (EU) information.
    /// </summary>
    public enum OpcDaEUType
    {
        /// <summary>
        /// No EU information available.
        /// </summary>
        NoEnum = OPCEUTYPE.OPC_NOENUM,

        /// <summary>
        /// Analog - the EU information will contain a SAFEARRAY of exactly two doubles (VT_ARRAY | VT_R8) corresponding to the LOW and HI EU range.
        /// </summary>
        Analog = OPCEUTYPE.OPC_ANALOG,

        /// <summary>
        /// Enumerated - the EU information will contain a SAFEARRAY of strings (VT_ARRAY | VT_BSTR) which contains a list of strings (Example: "OPEN", "CLOSE", "IN TRANSIT", etc.) corresponding to sequential numeric values (0, 1, 2, etc.)
        /// </summary>
        Enumerated = OPCEUTYPE.OPC_ENUMERATED
    }
}