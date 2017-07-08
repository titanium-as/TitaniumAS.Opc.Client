using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA server interface.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IOpcDaServer : IDisposable
    {
        /// <summary>
        /// Gets the server URI. The URL follows a template: opcda://host/progIdOrCLSID.
        /// </summary>
        /// <value>
        /// The server URI.
        /// </value>
        Uri Uri { get; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        /// <value>
        /// The client name.
        /// </value>
        string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the current culture for the server.
        /// </summary>
        /// <value>
        /// The current culture for the server.
        /// </value>
        CultureInfo Culture { get; set; }

        /// <summary>
        /// Queries available cultures of the server.
        /// </summary>
        /// <returns>Array of available cultures.</returns>
        CultureInfo[] QueryAvailableCultures();

        /// <summary>
        /// Gets the server description.
        /// </summary>
        /// <value>
        /// The server description.
        /// </value>
        OpcServerDescription ServerDescription { get; }

        /// <summary>
        /// Gets the server status.
        /// </summary>
        /// <returns>The server status.</returns>
        OpcDaServerStatus GetStatus();

        /// <summary>
        /// Gets the groups of the server.
        /// </summary>
        /// <value>
        /// The server groups.
        /// </value>
        ReadOnlyCollection<OpcDaGroup> Groups { get; }

        /// <summary>
        /// Adds the server group by group name and group state (optional).
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="state">The group state.</param>
        /// <returns>Added group.</returns>
        OpcDaGroup AddGroup(string name, OpcDaGroupState state = null);

        /// <summary>
        /// Removes the group from the server.
        /// </summary>
        /// <param name="group">The server group.</param>
        void RemoveGroup(OpcDaGroup group);

        /// <summary>
        /// Gets or attachs arbitrary user data to the server.
        /// </summary>
        /// <value>
        /// The user data.
        /// </value>
        object UserData { get; set; }

        /// <summary>
        /// Gets a value indicating whether the server instance is connected to COM server.
        /// </summary>
        /// <value>
        /// <c>true</c> if the server instance is connected to COM server; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the actual COM object.
        /// </summary>
        /// <value>
        /// The actual COM object.
        /// </value>
        object ComObject { get; }

        /// <summary>
        /// Asynchronously connects to the OpcDa server
        /// </summary>
        /// <returns>A task to wait upon</returns>
        Task ConnectAsync();

        /// <summary>
        /// Releases connection to COM server for the server instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Writes one or more values, qualities and timestamps for the items specified. If a client attempts to write VQ, VT, or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="itemIds">The server item identifiers.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        HRESULT[] WriteVQT(IList<string> itemIds, IList<OpcDaVQT> values);


        /// <summary>
        /// Reads one or more values, qualities and timestamps for the items specified using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="itemIds">The server item identifiers.</param>
        /// <param name="maxAge">The list of MaxAges for the server items.</param>
        /// <returns>The values of the server items.</returns>
        OpcDaVQTE[] Read(IList<string> itemIds, IList<TimeSpan> maxAge);

        /// <summary>
        /// Occurs when OPC server shutdowns right after connection to COM server has releases.
        /// </summary>
        event EventHandler<OpcShutdownEventArgs> Shutdown;

        /// <summary>
        /// Occurs when the server either connects or releases connection to COM server.
        /// </summary>
        event EventHandler<OpcDaServerConnectionStateChangedEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Occurs when the server groups have changed.
        /// </summary>
        event EventHandler<OpcDaServerGroupsChangedEventArgs> GroupsChanged;

        /// <summary>
        /// Synchronizes the server groups. It means existing groups will be replaced with new groups. It fires GroupsChanged event after synchronization.
        /// </summary>
        void SyncGroups();
    }
}
