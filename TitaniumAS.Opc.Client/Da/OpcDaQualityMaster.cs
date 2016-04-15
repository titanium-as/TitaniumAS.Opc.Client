namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a quality master status of specific item.
    /// </summary>
    public enum OpcDaQualityMaster : short
    {
        /// <summary>
        /// Value is not useful for reasons indicated by the Substatus.
        /// </summary>
        Bad = (short) 0,

        /// <summary>
        /// The quality of the value is uncertain for reasons indicated by the Substatus.
        /// </summary>
        Uncertain = (short) 64,

        /// <summary>
        /// Not used by OPC.
        /// </summary>
        Error = (short) 128,

        /// <summary>
        /// The Quality of the value is Good.
        /// </summary>
        Good = Error | Uncertain,
    }
}