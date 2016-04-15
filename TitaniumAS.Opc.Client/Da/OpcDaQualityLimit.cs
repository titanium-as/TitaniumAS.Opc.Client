namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents a limit field of quality status of specific item.
    /// </summary>
    public enum OpcDaQualityLimit : short
    {
        /// <summary>
        /// The value is free to move up or down. Servers which do not support limit should return 0.
        /// </summary>
        NotLimited = 0,

        /// <summary>
        /// The value has 'pegged' at some lower limit.
        /// </summary>
        LowLimited = 1,

        /// <summary>
        /// The value has 'pegged' at some high limit.
        /// </summary>
        HighLimited = 2,

        /// <summary>
        /// The value is a constant and cannot move.
        /// </summary>
        Constant = HighLimited | LowLimited,
    }
}