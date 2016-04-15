using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Determines single OPC DA item access rights. It intended to indicate whether the item is inherently readable or writable. For example a value representing a physical input would generally be readable but not writable. A value representing a physical output or an adjustable parameter such as a setpoint or alarm limit would generally be readable and writable. It is possible that a value representing a physical output with no readback capability might be marked writable but not readable. It is recommended that Client applications use this information only as something to be viewed by the user. Attempts by the user to read or write a value should always be passed by the client program to the server regardless of the access rights that were returned when the item was added. The Server can return HRESULT.OPC_E_BADRIGHTS if needed. Also, the returned Access Rights value is not related to security issues. It is expected that a server implementing security would validate any reads or writes for the currently logged in user as they occurred and in case of a problem would return an appropriate vendor specific HRESULT in response to that read or write.
    /// </summary>
    [Flags]
    public enum OpcDaAccessRights
    {
        /// <summary>
        /// Ignore access rights.
        /// </summary>
        Ignore = 0,

        /// <summary>
        /// Read only.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Write only.
        /// </summary>
        Write = 2,

        /// <summary>
        /// Read and write.
        /// </summary>
        ReadWrite = 3
    }
}