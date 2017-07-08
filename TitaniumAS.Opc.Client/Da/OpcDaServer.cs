using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Common.Logging;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Common.Internal;
using TitaniumAS.Opc.Client.Common.Wrappers;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Da.Wrappers;
using TitaniumAS.Opc.Client.Interop.Common;
using TitaniumAS.Opc.Client.Interop.Helpers;
using TitaniumAS.Opc.Client.Interop.System;
using System.Threading;
using System.Threading.Tasks;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    ///     Represents the OPC DA server.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.IOpcDaServer" />
    public class OpcDaServer : IOpcDaServer
    {
        private static readonly ILog Log = LogManager.GetLogger<OpcDaServer>();
        private readonly List<OpcDaGroup> _groups = new List<OpcDaGroup>();
        private readonly ConnectionPoint<IOPCShutdown> _shutdownConnectionPoint;
        private string _clientName;
        private bool _disposed;
        private OpcServerDescription _serverDescription;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OpcDaServer" /> class.
        /// </summary>
        /// <param name="uri">The server URI.</param>
        public OpcDaServer(Uri uri)
        {
            UrlValidator.CheckOpcUrl(uri);

            Uri = uri;
            ComProxyBlanket = ComProxyBlanket.Default;

            var shutdown = new OpcShutdown();
            shutdown.Shutdown += OnShutdown;
            ComWrapper.RpcFailed += OnRpcFailed;

            _shutdownConnectionPoint = new ConnectionPoint<IOPCShutdown>(shutdown);
            _clientName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaServer"/> class.
        /// </summary>
        /// <param name="progIdOrClsid">The OPC server programmatic identifier or class identifier.</param>
        /// <param name="host">The OPC server host.</param>
        public OpcDaServer(string progIdOrClsid, string host = null)
            : this(UrlBuilder.Build(progIdOrClsid, host))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaServer"/> class.
        /// </summary>
        /// <param name="clsid">The OPC server class identifier.</param>
        /// <param name="host">The OPC server host.</param>
        public OpcDaServer(Guid clsid, string host = null)
            : this(UrlBuilder.Build(clsid, host))
        {
        }

        /// <summary>
        /// Gets the COM proxy blanket of this instance.
        /// </summary>
        /// <value>
        /// The COM proxy blanket.
        /// </value>
        public ComProxyBlanket ComProxyBlanket { get; set; }

        /// <summary>
        /// Gets the actual COM object.
        /// </summary>
        /// <value>
        /// The actual COM object.
        /// </value>
        public object ComObject { get; private set; }

        /// <summary>
        ///     Gets the server description.
        /// </summary>
        /// <value>
        ///     The server description.
        /// </value>
        public OpcServerDescription ServerDescription
        {
            get
            {
                if (_serverDescription == null)
                {
                    var enumerator = new OpcServerEnumeratorAuto();
                    _serverDescription = UrlParser.Parse(
                        Uri,
                        (host, progId) => enumerator.GetServerDescription(host, progId),
                        (host, clsid) => enumerator.GetServerDescription(host, clsid));
                }
                return _serverDescription;
            }
        }

        /// <summary>
        /// Connects the server instance to COM server.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Already connected to the OPC DA server.</exception>
        public void Connect()
        {
            if (IsConnected)
                throw new InvalidOperationException("Already connected to the OPC DA server.");

            Log.TraceFormat("Connecting to '{0}' opc server", Uri);

            var enumerator = new OpcServerEnumeratorAuto();
            Tuple<string, Guid> hostAndCLSID = UrlParser.Parse(
                Uri,
                (host, progId) => new Tuple<string, Guid>(host, enumerator.CLSIDFromProgId(progId, host)),
                (host, clsid) => new Tuple<string, Guid>(host, clsid));

            ComObject = Com.CreateInstanceWithBlanket(hostAndCLSID.Item2, hostAndCLSID.Item1, null, ComProxyBlanket);
            _shutdownConnectionPoint.TryConnect(ComObject);

            Log.TraceFormat("Connected to '{0}' opc server.", Uri);
            try
            {
                ClientName = _clientName;
            }
            catch (Exception ex)
            {
                Log.Warn("Cannot setup name of client.", ex);
            }
            OnConnectionStateChanged(true);
        }

        /// <summary>
        /// Asynchronout connect method
        /// </summary>
        /// <returns>
        /// A running task to wait on for connect
        /// </returns>
        /// <seealso cref="Connect()"/>
        public Task ConnectAsync()
        {
            return Task.Factory.StartNew(() => { Connect(); });
        }

        private void DisconnectImpl(bool rpcFailed = false)
        {
            if (!IsConnected)
                return;

            Log.TraceFormat("Disconnecting from '{0}' opc server", Uri);
            if (!rpcFailed)
            {
                _shutdownConnectionPoint.Disconnect();
            }
            RemoveAllGroups(rpcFailed);

            if (ComObject != null)
            {
                try
                {
                    ComObject.ReleaseComServer();
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to release opc server's COM object", ex);
                }
            }

            ComObject = null;
            Log.TraceFormat("Disconnected from '{0}' opc server", Uri);
            OnConnectionStateChanged(false);
        }


        /// <summary>
        ///     Releases connection to COM server for the server instance.
        /// </summary>
        public void Disconnect()
        {
            DisconnectImpl();
        }

        /// <summary>
        ///     Gets the server URI. The URL follows a template: opcda://host/progIdOrCLSID.
        /// </summary>
        /// <value>
        ///     The server URI.
        /// </value>
        public Uri Uri { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether the server instance is connected to COM server.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the server instance is connected to COM server; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get { return ComObject != null; }
        }

        /// <summary>
        /// Gets or sets the current culture for the server.
        /// </summary>
        /// <value>
        /// The current culture for the server.
        /// </value>
        public CultureInfo Culture
        {
            get
            {
                CheckConnected();
                int localeId = As<OpcCommon>().LocaleID;
                return CultureHelper.GetCultureInfo(localeId);
            }
            set
            {
                CheckConnected();
                As<OpcCommon>().LocaleID = CultureHelper.GetLocaleId(value);
            }
        }

        /// <summary>
        /// Queries available cultures of the server.
        /// </summary>
        /// <returns>
        /// Array of available cultures.
        /// </returns>
        public CultureInfo[] QueryAvailableCultures()
        {
            CheckConnected();
            return As<OpcCommon>().QueryAvailableLocaleIDs().Select(CultureHelper.GetCultureInfo).ToArray();
        }

        /// <summary>
        ///     Gets or sets the client name.
        /// </summary>
        /// <value>
        ///     The client name.
        /// </value>
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                if (IsConnected)
                {
                    As<OpcCommon>().ClientName = value;
                }
            }
        }

        /// <summary>
        ///     Gets the groups of the server.
        /// </summary>
        /// <value>
        ///     The server groups.
        /// </value>
        public ReadOnlyCollection<OpcDaGroup> Groups
        {
            get { return _groups.AsReadOnly(); }
        }

        /// <summary>
        ///     Gets or attachs arbitrary user data to the server.
        /// </summary>
        /// <value>
        ///     The user data.
        /// </value>
        public object UserData { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Occurs when OPC server shutdowns right after connection to COM server has releases.
        /// </summary>
        public event EventHandler<OpcShutdownEventArgs> Shutdown;

        /// <summary>
        ///     Occurs when the server either connects or releases connection to COM server.
        /// </summary>
        public event EventHandler<OpcDaServerConnectionStateChangedEventArgs> ConnectionStateChanged;

        /// <summary>
        ///     Occurs when the server groups have changed.
        /// </summary>
        public event EventHandler<OpcDaServerGroupsChangedEventArgs> GroupsChanged;

        /// <summary>
        ///     Removes the group from the server.
        /// </summary>
        /// <param name="group">The server group.</param>
        /// <exception cref="System.ArgumentException">The group doesn't belong to the server;group</exception>
        public void RemoveGroup(OpcDaGroup group)
        {
            CheckConnected();

            if (group.Server != this)
                throw new ArgumentException("The group doesn't belong to the server", "group");
            TryRemoveGroup(@group);
        }

        /// <summary>
        ///     Gets the server status.
        /// </summary>
        /// <returns>
        ///     The server status.
        /// </returns>
        public OpcDaServerStatus GetStatus()
        {
            CheckConnected();
            return new OpcDaServerStatus(As<OpcServer>().GetStatus());
        }

        /// <summary>
        ///     Reads one or more values, qualities and timestamps for the items specified using MaxAge. If the information in the
        ///     cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the
        ///     device for the requested information.
        /// </summary>
        /// <param name="itemIds">The server item identifiers.</param>
        /// <param name="maxAge">The list of MaxAges for the server items.</param>
        /// <returns>
        ///     The values of the server items.
        /// </returns>
        public OpcDaVQTE[] Read(IList<string> itemIds, IList<TimeSpan> maxAge)
        {
            CheckConnected();
            return As<OpcItemIO>().Read(itemIds, maxAge);
        }

        /// <summary>
        ///     Writes one or more values, qualities and timestamps for the items specified. If a client attempts to write VQ, VT,
        ///     or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="itemIds">The server item identifiers.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        public HRESULT[] WriteVQT(IList<string> itemIds, IList<OpcDaVQT> values)
        {
            CheckConnected();
            return As<OpcItemIO>().WriteVQT(itemIds, values);
        }

        /// <summary>
        ///     Adds the server group by group name and group state (optional).
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="state">The group state.</param>
        /// <returns>
        ///     Added group.
        /// </returns>
        public OpcDaGroup AddGroup(string name, OpcDaGroupState state = null)
        {
            CheckConnected();

            if (state == null)
                state = new OpcDaGroupState();

            int serverGroupHandle;
            TimeSpan revisedUpdateRate;
            int localeId = CultureHelper.GetLocaleId(state.Culture);

            object opcDaGroup = As<OpcServer>().AddGroup(name,
                state.IsActive.GetValueOrDefault(false),
                state.UpdateRate.GetValueOrDefault(TimeSpan.FromSeconds(1)),
                state.ClientHandle.GetValueOrDefault(0),
                state.TimeBias,
                state.PercentDeadband,
                localeId, out serverGroupHandle, out revisedUpdateRate);

            OpcDaGroup @group = CreateGroupWrapper(opcDaGroup);
            @group.UserData = state.UserData;
            OnGroupsChanged(new OpcDaServerGroupsChangedEventArgs(group, null));
            return @group;
        }

        /// <summary>
        ///     Synchronizes the server groups. It means existing groups will be replaced with new groups. It fires GroupsChanged
        ///     event after synchronization.
        /// </summary>
        public void SyncGroups()
        {
            CheckConnected();
            List<OpcDaGroup> newGroups = EnumerateGroups();

            // Dispose old groups
            foreach (OpcDaGroup @group in _groups)
            {
                ((IDisposable) @group).Dispose();
                OnGroupsChanged(new OpcDaServerGroupsChangedEventArgs(null, @group));
            }
            _groups.Clear();

            // Add new groups
            _groups.InsertRange(0, newGroups);
            foreach (OpcDaGroup @group in _groups)
            {
                OnGroupsChanged(new OpcDaServerGroupsChangedEventArgs(@group, null));
            }
        }

        private void OnRpcFailed(object sender, RpcFailedEventArgs args)
        {
            if (args.UserData != this) return;
            try
            {
                DisconnectImpl(true);
            }
            catch (Exception ex)
            {
                Log.Error("Disconnect failed.",ex);
            }
            try
            {
                OnShutdown(new OpcShutdownEventArgs("RPC failed", args.Error));
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error during shutdown of '{0}' opc server.", ex, Uri);
            }
        }

        private void OnShutdown(object sender, OpcShutdownEventArgs args)
        {
            try
            {
                DisconnectImpl();
            }
            catch (Exception ex)
            {
                Log.Error("Disconnect failed.", ex);
            }

            try
            {
                OnShutdown(new OpcShutdownEventArgs(args.Reason, args.Error));
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error during shutdown of '{0}' opc server.", ex, Uri);
            }
        }

        /// <summary>
        ///     Try to cast this instance to specified COM wrapper type.
        /// </summary>
        /// <typeparam name="T">The COM wrapper type.</typeparam>
        /// <returns>The wrapped instance or null.</returns>
        public T As<T>() where T : ComWrapper
        {
            try
            {
                if (ComObject == null)
                    return default(T);
                return (T) Activator.CreateInstance(typeof (T), ComObject, this);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        ///     Determines whether this instance is of specified COM wrapper type.
        /// </summary>
        /// <typeparam name="T">The COM wrapper type.</typeparam>
        /// <returns><c>true</c> if this instance is of specified COM wrapper type; otherwise, <c>false</c>.</returns>
        public bool Is<T>() where T : ComWrapper
        {
            return As<T>() != null;
        }

        private void RemoveAllGroups(bool rpcFailed = false)
        {
            foreach (OpcDaGroup opcDaGroup in _groups.ToArray())
            {
                TryRemoveGroup(opcDaGroup,rpcFailed);
            }
            _groups.Clear();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Uri.ToString();
        }

        private void CheckConnected()
        {
            if (!IsConnected)
                throw ExceptionHelper.NotConnected();
        }

        private List<string> EnumerateGroupNames(OpcDaEnumScope scope = OpcDaEnumScope.All)
        {
            object enumeratorObj = As<OpcServer>().CreateGroupEnumerator((int) scope);
            var enumerator = (IEnumString) enumeratorObj;
            return enumerator.EnumareateAllAndRelease(OpcConfiguration.BatchSize);
        }

        private List<OpcDaGroup> EnumerateGroups(OpcDaEnumScope scope = OpcDaEnumScope.All)
        {
            object enumeratorObj = As<OpcServer>().CreateGroupEnumerator((int) scope);
            var enumerator = (IEnumUnknown) enumeratorObj;
            List<object> interfaces = enumerator.EnumareateAllAndRelease(OpcConfiguration.BatchSize);
            return interfaces.Select(i => new OpcDaGroup(i, this)).ToList();
        }

        private OpcDaGroup GetGroupByName(string name)
        {
            return new OpcDaGroup(As<OpcServer>().GetGroupByName(name), this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            DisconnectImpl();

            _disposed = true;
            Log.DebugFormat("Opc server '{0}' disposed.", Uri);
        }

        private bool TryRemoveGroup(OpcDaGroup @group, bool rpcFailed = false)
        {
            try
            {
                ((IDisposable) @group).Dispose();
                if (!rpcFailed)
                {
                    As<OpcServer>().RemoveGroup(@group.ServerHandle, false);
                }
                _groups.Remove(group);
                OnGroupsChanged(new OpcDaServerGroupsChangedEventArgs(null, group));
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Cannot remove group '{0}'.", ex, @group.Name);
                return false;
            }
        }

        ~OpcDaServer()
        {
            Dispose(false);
        }

        protected virtual void OnShutdown(OpcShutdownEventArgs args)
        {
            EventHandler<OpcShutdownEventArgs> handler = Shutdown;
            if (handler != null) handler(this, args);
        }

        internal OpcDaGroup CreateGroupWrapper(object opcDaGroup)
        {
            var group = new OpcDaGroup(opcDaGroup, this);
            _groups.Add(group);
            return group;
        }

        protected virtual void OnConnectionStateChanged(bool isConnected)
        {
            EventHandler<OpcDaServerConnectionStateChangedEventArgs> handler = ConnectionStateChanged;
            if (handler != null) handler(this, new OpcDaServerConnectionStateChangedEventArgs(isConnected));
        }

        protected virtual void OnGroupsChanged(OpcDaServerGroupsChangedEventArgs e)
        {
            EventHandler<OpcDaServerGroupsChangedEventArgs> handler = GroupsChanged;
            if (handler != null) handler(this, e);
        }
    }
}