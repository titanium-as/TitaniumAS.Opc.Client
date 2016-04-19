namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents features of an OPC DA group.
    /// </summary>
    public enum OpcDaGroupFeatures
    {
        /// <summary>
        /// Can be read.
        /// </summary>
        Read,

        /// <summary>
        /// Can be asynchronously read.
        /// </summary>
        ReadAsync,

        /// <summary>
        /// Can be read using MaxAge.
        /// </summary>
        ReadMaxAge,

        /// <summary>
        /// Can be asynchronously read using MaxAge.
        /// </summary>
        ReadMaxAgeAsync,

        /// <summary>
        /// Can be asynchronously refreshed.
        /// </summary>
        RefreshAsync,

        /// <summary>
        /// Can be asynchronously refreshed using MaxAge.
        /// </summary>
        RefreshMaxAgeAsync,

        /// <summary>
        /// Can be written.
        /// </summary>
        Write,

        /// <summary>
        /// Can be asynchronously written.
        /// </summary>
        WriteAsync,

        /// <summary>
        /// Can write VQT.
        /// </summary>
        WriteVQT,

        /// <summary>
        /// Can asynchronously write VQT.
        /// </summary>
        WriteVQTAsync,

        /// <summary>
        /// Can use subscriptions.
        /// </summary>
        Subscription,

        /// <summary>
        /// Can use keep-alive callbacks.
        /// </summary>
        KeepAlive,

        /// <summary>
        /// Can use sampling.
        /// </summary>
        Sampling
    }
}