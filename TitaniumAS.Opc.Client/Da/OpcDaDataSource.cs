using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents data source to retrieving data.
    /// </summary>
    public enum OpcDaDataSource
    {
        /// <summary>
        /// The cache.
        /// </summary>
        Cache = OPCDATASOURCE.OPC_DS_CACHE,

        /// <summary>
        /// The device.
        /// </summary>
        Device = OPCDATASOURCE.OPC_DS_DEVICE
    }
}