using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the current OPC DA server status.
    /// </summary>
    public enum OpcDaServerState
    {
        /// <summary>
        /// The server is running normally.
        /// </summary>
        Running = OPCSERVERSTATE.OPC_STATUS_RUNNING,

        /// <summary>
        /// A vendor specific fatal error has occurred within the server. The server is no longer functioning. The recovery procedure from the situation is vendor specific. E_FAIL error code should be returned from any other server method.
        /// </summary>
        Failed = OPCSERVERSTATE.OPC_STATUS_FAILED,

        /// <summary>
        /// The server is running but has no configuration loaded and thus cannot run normally. This state implies the server needs configuration information in order to run normally. Servers which do not require configuration should not return this state.
        /// </summary>
        NoConfig = OPCSERVERSTATE.OPC_STATUS_NOCONFIG,

        /// <summary>
        /// The server has been temporarily suspended and is not getting or sending data.
        /// </summary>
        Suspended = OPCSERVERSTATE.OPC_STATUS_SUSPENDED,

        /// <summary>
        /// The server is in Test Mode. The outputs are disconnected from the real hardware but the server will otherwise behave normally. Inputs may be real or may be simulated depending on the vendor implementation. Quality will generally be returned normally.
        /// </summary>
        Test = OPCSERVERSTATE.OPC_STATUS_TEST,

        /// <summary>
        /// The server is running properly but is having difficulty accessing data from its data sources. This may be due to communication problems, or some other problem preventing the underlying device, control system, etc. from returning valid data. It may be complete failure, meaning that no data is available, or a partial failure, meaning that some data is still available. It is expected that items affected by the fault will individually return with a BAD quality indication for the items.
        /// </summary>
        CommFault = OPCSERVERSTATE.OPC_STATUS_COMM_FAULT
    }
}