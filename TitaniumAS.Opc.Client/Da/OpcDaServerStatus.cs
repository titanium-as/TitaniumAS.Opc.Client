using System;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents OPC DA server status.
    /// </summary>
    public class OpcDaServerStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaServerStatus"/> class.
        /// </summary>
        public OpcDaServerStatus()
        {
        }

        internal OpcDaServerStatus(OPCSERVERSTATUS opcserverstatus)
        {
            StartTime = FileTimeConverter.FromFileTime(opcserverstatus.ftStartTime);
            CurrentTime = FileTimeConverter.FromFileTime(opcserverstatus.ftCurrentTime);
            LastUpdateTime = FileTimeConverter.FromFileTime(opcserverstatus.ftLastUpdateTime);
            ServerState = (OpcDaServerState) opcserverstatus.dwServerState;
            GroupCount = opcserverstatus.dwGroupCount;
            Bandwidth = opcserverstatus.dwBandWidth;
            Version = new Version(opcserverstatus.wMajorVersion, opcserverstatus.wMinorVersion,
                opcserverstatus.wBuildNumber);
            Reserved = opcserverstatus.wReserved;
            VendorInfo = opcserverstatus.szVendorInfo;
        }

        /// <summary>
        /// Gets or sets the server start time. It is constant for the server instance and is not reset when the server changes states.
        /// </summary>
        /// <value>
        /// The server start time.
        /// </value>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the current time from the server point of view.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public DateTimeOffset CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets the last update time. The time when the server instance sent the last data value update to the client.
        /// </summary>
        /// <value>
        /// The last update time.
        /// </value>
        public DateTimeOffset LastUpdateTime { get; set; }

        /// <summary>
        /// Gets or sets current server status.
        /// </summary>
        /// <value>
        /// The current server status.
        /// </value>
        public OpcDaServerState ServerState { get; set; }

        /// <summary>
        /// Gets or sets the total number of groups in the server instance.
        /// </summary>
        /// <value>
        /// The total number of groups.
        /// </value>
        public int GroupCount { get; set; }

        /// <summary>
        /// Gets or sets the approximate percent of bandwidth currently in use by the server. It may be 0xFFFFFFFF if the bandwidth is unknown.
        /// </summary>
        /// <value>
        /// The percent of bandwidth.
        /// </value>
        public int Bandwidth { get; set; }

        /// <summary>
        /// Gets or sets the version of the server software.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public Version Version { get; set; }

        public short Reserved { get; set; }

        /// <summary>
        /// Gets or sets the vendor specific information about the server. For example the name of the company and the type of devices supported by the server.
        /// </summary>
        /// <value>
        /// The vendor specific information about the server.
        /// </value>
        public string VendorInfo { get; set; }
    }
}