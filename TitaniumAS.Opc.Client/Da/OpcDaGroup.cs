using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da.Internal;
using TitaniumAS.Opc.Client.Da.Internal.Requests;
using TitaniumAS.Opc.Client.Da.Wrappers;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;
using TitaniumAS.Opc.Client.Interop.System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    ///     Represents OPC DA group.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="TitaniumAS.Opc.Da.IOpcDaGroup" />
    public class OpcDaGroup : IDisposable, IOpcDaGroup
    {
        private static readonly ILog Log = LogManager.GetLogger<OpcDaGroup>();
        private readonly List<OpcDaItem> _items = new List<OpcDaItem>();
        private AsyncRequestManager _asyncRequestManager;
        private int _clientHandle;
        private bool _disposed;
        private bool _isActive;
        private TimeSpan _keepAlive = TimeSpan.Zero;
        private CultureInfo _culture;
        private string _name;
        private float _percentDeadband;
        private int _serverHandle;
        private TimeSpan _timeBias;
        private TimeSpan _updateRate;

        internal OpcDaGroup(object groupComObject, OpcDaServer opcDaServer)
        {
            ComObject = groupComObject;
            Server = opcDaServer;
            SyncState();
            _asyncRequestManager = new AsyncRequestManager(this);
            _asyncRequestManager.NewItemValues += values => OnValuesChanged(new OpcDaItemValuesChangedEventArgs(values));
        }

        /// <summary>
        /// Gets the actual COM object.
        /// </summary>
        /// <value>
        /// The actual COM object.
        /// </value>
        public object ComObject { get; private set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Occurs when the group has destroyed.
        /// </summary>
        public event EventHandler Destroyed;

        /// <summary>
        ///     Occurs when the group items changed.
        /// </summary>
        public event EventHandler<OpcDaItemsChangedEventArgs> ItemsChanged;

        /// <summary>
        ///     Occurs when the values of the group items changed.
        /// </summary>
        public event EventHandler<OpcDaItemValuesChangedEventArgs> ValuesChanged;

        /// <summary>
        ///     Reads the specified group items asynchronously.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> ReadAsync(IList<OpcDaItem> items, CancellationToken token)
        {
            CheckSupported(OpcDaGroupFeatures.ReadAsync);
            CheckItems(items);
            var request = new ReadAsyncRequest(As<OpcAsyncIO2>());
            _asyncRequestManager.AddRequest(request);
            return request.Start(items, token);
        }

        /// <summary>
        ///     Reads the specified group items asynchronously.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> ReadAsync(IList<OpcDaItem> items)
        {
            return ReadAsync(items, new CancellationToken());
        }

        /// <summary>
        ///     Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge,
        ///     then the data will be obtained from the cache, otherwise the server must access the device for the requested
        ///     information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The MaxAge for all items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, TimeSpan maxAge,
            CancellationToken token)
        {
            TimeSpan[] maxAges = ArrayHelpers.CreateMaxAges(items.Count, maxAge);
            return ReadMaxAgeAsync(items, maxAges, token);
        }

        /// <summary>
        ///     Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge,
        ///     then the data will be obtained from the cache, otherwise the server must access the device for the requested
        ///     information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The MaxAge for all items.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, TimeSpan maxAge)
        {
            return ReadMaxAgeAsync(items, maxAge, new CancellationToken());
        }

        /// <summary>
        ///     Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge,
        ///     then the data will be obtained from the cache, otherwise the server must access the device for the requested
        ///     information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The list of MaxAges for the group items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">maxAge</exception>
        /// <exception cref="System.ArgumentException">Invalid size of maxAge.;maxAge</exception>
        public Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, IList<TimeSpan> maxAge,
            CancellationToken token)
        {
            CheckItems(items);
            CheckSupported(OpcDaGroupFeatures.ReadMaxAgeAsync);

            if (maxAge == null)
                throw new ArgumentNullException("maxAge");

            if (items.Count != maxAge.Count)
                throw new ArgumentException("Invalid size of maxAge.", "maxAge");


            var request = new ReadMaxAgeAsyncRequest(As<OpcAsyncIO3>());
            _asyncRequestManager.AddRequest(request);
            return request.Start(items, maxAge, token);
        }

        /// <summary>
        ///     Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge,
        ///     then the data will be obtained from the cache, otherwise the server must access the device for the requested
        ///     information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The list of MaxAges for the group items.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, IList<TimeSpan> maxAge)
        {
            return ReadMaxAgeAsync(items, maxAge, new CancellationToken());
        }

        /// <summary>
        ///     Asynchronously refreshes all active group items (whether they have changed or not). Inactive items are not included
        ///     in the callback.
        /// </summary>
        /// <param name="dataSource">The data source either cache or device.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> RefreshAsync(OpcDaDataSource dataSource, CancellationToken token)
        {
            CheckSupported(OpcDaGroupFeatures.RefreshAsync);

            var request = new RefreshAsyncRequest(As<OpcAsyncIO2>());
            _asyncRequestManager.AddRequest(request);
            return request.Start(dataSource, token);
        }

        /// <summary>
        ///     Asynchronously refreshes all active group items (whether they have changed or not). Inactive items are not included
        ///     in the callback.
        /// </summary>
        /// <param name="dataSource">The data source either cache or device.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> RefreshAsync(OpcDaDataSource dataSource = OpcDaDataSource.Cache)
        {
            return RefreshAsync(dataSource, new CancellationToken());
        }

        /// <summary>
        ///     Asynchronously refreshes all active group items (whether they have changed or not) using MaxAge. Inactive items are
        ///     not included in the callback. Some of the values may be obtained from cache while others could be obtained from the
        ///     device depending on the "freshness" of the data in the cache.
        /// </summary>
        /// <param name="maxAge">The MaxAge value, which will determine the MaxAge for all active items in the group.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> RefreshMaxAgeAsync(TimeSpan maxAge, CancellationToken token)
        {
            CheckSupported(OpcDaGroupFeatures.RefreshMaxAgeAsync);

            var request = new RefreshMaxAgeAsyncRequest(As<OpcAsyncIO3>());
            _asyncRequestManager.AddRequest(request);
            return request.Start(maxAge, token);
        }

        /// <summary>
        ///     Asynchronously refreshes all active group items (whether they have changed or not) using MaxAge. Inactive items are
        ///     not included in the callback. Some of the values may be obtained from cache while others could be obtained from the
        ///     device depending on the "freshness" of the data in the cache.
        /// </summary>
        /// <param name="maxAge">The MaxAge value, which will determine the MaxAge for all active items in the group.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public Task<OpcDaItemValue[]> RefreshMaxAgeAsync(TimeSpan maxAge)
        {
            return RefreshMaxAgeAsync(maxAge, new CancellationToken());
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the group is subscribed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the group is subscribed; otherwise, <c>false</c>.
        /// </value>
        public bool IsSubscribed
        {
            get
            {
                if (!IsSupported(OpcDaGroupFeatures.Subscription))
                    return false;
                return _asyncRequestManager.IsConnected && As<OpcAsyncIO2>().Enable;
            }
            set
            {
                if (!IsSupported(OpcDaGroupFeatures.Subscription) || !_asyncRequestManager.IsConnected)
                    return;
                As<OpcAsyncIO2>().Enable = value;
            }
        }

        /// <summary>
        ///     Asynchronously writes values to one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values to be written to the items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        public Task<HRESULT[]> WriteAsync(IList<OpcDaItem> items, IList<object> values,
            CancellationToken token)
        {
            CheckItems(items);
            CheckSupported(OpcDaGroupFeatures.WriteAsync);

            var request = new WriteAsyncRequest(As<OpcAsyncIO2>());
            _asyncRequestManager.AddRequest(request);
            return request.Start(items, values, token);
        }

        /// <summary>
        ///     Asynchronously writes values to one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values to be written to the items.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        public Task<HRESULT[]> WriteAsync(IList<OpcDaItem> items, IList<object> values)
        {
            CheckSupported(OpcDaGroupFeatures.WriteAsync);
            return WriteAsync(items, values, new CancellationToken());
        }

        /// <summary>
        ///     Asynchronously writes one or more values, qualities and timestamps for the items specified. If a client attempts to
        ///     write VQ, VT, or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        public Task<HRESULT[]> WriteVQTAsync(IList<OpcDaItem> items, IList<OpcDaVQT> values,
            CancellationToken token)
        {
            CheckItems(items);
            CheckSupported(OpcDaGroupFeatures.WriteVQTAsync);

            var request = new WriteVQTAsyncRequest(As<OpcAsyncIO3>());
            _asyncRequestManager.AddRequest(request);
            OPCITEMVQT[] vqts = ArrayHelpers.CreateOpcItemVQT(values);
            return request.Start(items, vqts, token);
        }

        /// <summary>
        ///     Asynchronously writes one or more values, qualities and timestamps for the items specified. If a client attempts to
        ///     write VQ, VT, or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        public Task<HRESULT[]> WriteVQTAsync(IList<OpcDaItem> items, IList<OpcDaVQT> values)
        {
            return WriteVQTAsync(items, values, new CancellationToken());
        }

        /// <summary>
        ///     Gets the items of the group.
        /// </summary>
        /// <value>
        ///     The group.
        /// </value>
        public ReadOnlyCollection<OpcDaItem> Items
        {
            get { return _items.AsReadOnly(); }
        }

        /// <summary>
        ///     Gets or sets the update rate requested for the group (milliseconds).
        /// </summary>
        /// <value>
        ///     The update rate of the group (milliseconds).
        /// </value>
        public TimeSpan UpdateRate
        {
            get { return _updateRate; }
            set { SetState(new OpcDaGroupState {UpdateRate = value}); }
        }

        /// <summary>
        ///     Gets the server handle of the group.
        /// </summary>
        /// <value>
        ///     The server handle of the group.
        /// </value>
        public int ServerHandle
        {
            get { return _serverHandle; }
        }

        /// <summary>
        ///     Gets or sets the client handle of the group.
        /// </summary>
        /// <value>
        ///     The client handle.
        /// </value>
        public int ClientHandle
        {
            get { return _clientHandle; }
            set { SetState(new OpcDaGroupState {ClientHandle = value}); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the group is active.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the group is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return _isActive; }
            set { SetState(new OpcDaGroupState {IsActive = value}); }
        }

        /// <summary>
        ///     Gets or sets the TimeZone Bias of the group (in minutes).
        /// </summary>
        /// <value>
        ///     The TimeZone Bias of the group (in minutes).
        /// </value>
        public TimeSpan TimeBias
        {
            get { return _timeBias; }
            set { SetState(new OpcDaGroupState {TimeBias = value}); }
        }

        /// <summary>
        ///     Gets or sets the range of the Deadband is from 0.0 to 100.0 Percent.
        /// </summary>
        /// <value>
        ///     The percent deadband.
        /// </value>
        public float PercentDeadband
        {
            get { return _percentDeadband; }
            set { SetState(new OpcDaGroupState {PercentDeadband = value}); }
        }

        /// <summary>
        /// Gets or sets the current culture for the group.
        /// </summary>
        /// <value>
        /// The current culture for the group.
        /// </value>
        public CultureInfo Culture
        {
            get { return _culture; }
            set { SetState(new OpcDaGroupState {Culture = value}); }
        }

        /// <summary>
        ///     Gets or sets the name of the group.
        /// </summary>
        /// <value>
        ///     The name of the group.
        /// </value>
        public string Name
        {
            get { return _name; }
            set
            {
                As<OpcGroupStateMgt>().SetName(value);
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the maximum interval (in milliseconds) between subscription callbacks. A zero value indicates the
        ///     client does not wish to receive any empty keep-alive callbacks.
        /// </summary>
        /// <value>
        ///     The keep alive time (in milliseconds).
        /// </value>
        public TimeSpan KeepAlive
        {
            get
            {
                if (!Is<OpcGroupStateMgt2>())
                    return TimeSpan.Zero;
                return _keepAlive;
            }
            set
            {
                if (!Is<OpcGroupStateMgt2>())
                    return;
                _keepAlive = As<OpcGroupStateMgt2>().SetKeepAlive(value);
            }
        }

        /// <summary>
        ///     Gets the server to which the group belongs to.
        /// </summary>
        /// <value>
        ///     The server to which the group belongs to.
        /// </value>
        public OpcDaServer Server { get; private set; }

        /// <summary>
        ///     Gets or attachs arbitrary user data to the group.
        /// </summary>
        /// <value>
        ///     The user data.
        /// </value>
        public object UserData { get; set; }

        /// <summary>
        ///     Reads the specified group items.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public OpcDaItemValue[] Read(IList<OpcDaItem> items, OpcDaDataSource dataSource = OpcDaDataSource.Cache)
        {
            CheckItems(items);  
            CheckSupported(OpcDaGroupFeatures.Read);
            int[] serverHandles = ArrayHelpers.GetServerHandles(items);

            HRESULT[] ppErrors;
            OPCITEMSTATE[] ppItemValues = As<OpcSyncIO>().Read((OPCDATASOURCE) dataSource, serverHandles, out ppErrors);
            OpcDaItemValue[] result = OpcDaItemValue.Create(this, ppItemValues, ppErrors);
            OnValuesChanged(new OpcDaItemValuesChangedEventArgs(result));
            return result;
        }

        /// <summary>
        ///     Writes values to one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values to be written to the items.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        public HRESULT[] Write(IList<OpcDaItem> items, object[] values)
        {
            CheckItems(items);
            CheckSupported(OpcDaGroupFeatures.Write);
            int[] serverHandles = ArrayHelpers.GetServerHandles(items);
            return As<OpcSyncIO>().Write(serverHandles, values);
        }

        /// <summary>
        ///     Reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data
        ///     will be obtained from the cache, otherwise the server must access the device for the requested information.
        ///     //In addition, the ability to Read from a group based on a "MaxAge" is provided.
        ///     //from cache or device as determined by "staleness" of cache data
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The MaxAge for all items.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        public OpcDaItemValue[] ReadMaxAge(IList<OpcDaItem> items, TimeSpan maxAge)
        {
            CheckItems(items);
            CheckSupported(OpcDaGroupFeatures.ReadMaxAge);
            TimeSpan[] maxAges = ArrayHelpers.CreateMaxAges(items.Count, maxAge);
            return ReadMaxAge(items, maxAges);
        }

        /// <summary>
        ///     Determines whether the specified feature is supported.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">feature;null</exception>
        public bool IsSupported(OpcDaGroupFeatures feature)
        {
            switch (feature)
            {
                case OpcDaGroupFeatures.Read:
                case OpcDaGroupFeatures.Write:
                    return Is<OpcSyncIO>();
                case OpcDaGroupFeatures.ReadAsync:
                case OpcDaGroupFeatures.WriteAsync:
                case OpcDaGroupFeatures.RefreshAsync:
                case OpcDaGroupFeatures.Subscription:
                    return Is<OpcAsyncIO2>();
                case OpcDaGroupFeatures.ReadMaxAge:
                case OpcDaGroupFeatures.WriteVQT:
                    return Is<OpcSyncIO2>();
                case OpcDaGroupFeatures.ReadMaxAgeAsync:
                case OpcDaGroupFeatures.RefreshMaxAgeAsync:
                case OpcDaGroupFeatures.WriteVQTAsync:
                    return Is<OpcAsyncIO3>();
                case OpcDaGroupFeatures.KeepAlive:
                    return Is<OpcItemMgt>();
                case OpcDaGroupFeatures.Sampling:
                    return Is<OpcItemSamplingMgt>();
                default:
                    throw new ArgumentOutOfRangeException("feature", feature, null);
            }
        }

        /// <summary>
        ///     Reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data
        ///     will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The list of MaxAges for the group items.</param>
        /// <returns>
        ///     The values of the group items.
        /// </returns>
        /// <exception cref="System.ArgumentException">Invalid size of maxAge;maxAge</exception>
        public OpcDaItemValue[] ReadMaxAge(IList<OpcDaItem> items, IList<TimeSpan> maxAge)
        {
            CheckSupported(OpcDaGroupFeatures.ReadMaxAge);
            CheckItems(items);

            int[] serverHandles = ArrayHelpers.GetServerHandles(items);

            if (serverHandles.Length != maxAge.Count)
                throw new ArgumentException("Invalid size of maxAge", "maxAge");

//            int[] intMaxAge = ArrayHelpers.CreateMaxAgeArray(maxAge, items.Count);

            DateTimeOffset[] timestamps;
            HRESULT[] errors;
            OpcDaQuality[] qualities;
            object[] ppvValues = As<OpcSyncIO2>()
                .ReadMaxAge(serverHandles, maxAge, out qualities, out timestamps, out errors);

            OpcDaItemValue[] result = OpcDaItemValue.Create(items, ppvValues, qualities, timestamps, errors);
            OnValuesChanged(new OpcDaItemValuesChangedEventArgs(result));
            return result;
        }

        /// <summary>
        ///     Writes one or more values, qualities and timestamps for the items specified. If a client attempts to write VQ, VT,
        ///     or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <returns>
        ///     Array of HRESULTs indicating the success of the individual item Writes.
        /// </returns>
        /// <exception cref="System.ArgumentException">Invalid size of values;values</exception>
        public HRESULT[] WriteVQT(IList<OpcDaItem> items, IList<OpcDaVQT> values)
        {
            CheckSupported(OpcDaGroupFeatures.WriteVQT);
            CheckItems(items);
            int[] serverHandles = ArrayHelpers.GetServerHandles(items);

            if (serverHandles.Length != values.Count)
                throw new ArgumentException("Invalid size of values", "values");

            OPCITEMVQT[] vqts = ArrayHelpers.CreateOpcItemVQT(values);
            return As<OpcSyncIO2>().WriteVQT(serverHandles, vqts);
        }

        /// <summary>
        ///     Add one or more items to the group.
        /// </summary>
        /// <param name="itemDefinitions">The list of item definitions.</param>
        /// <returns>
        ///     Array of item results.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">itemDefinitions</exception>
        public OpcDaItemResult[] AddItems(IList<OpcDaItemDefinition> itemDefinitions)
        {
            if (itemDefinitions == null)
                throw new ArgumentNullException("itemDefinitions");
            if (itemDefinitions.Count == 0)
                return new OpcDaItemResult[0];

            OPCITEMDEF[] pItemArray = ArrayHelpers.CreateOPITEMDEFs(itemDefinitions);

            HRESULT[] ppErrors;
            OPCITEMRESULT[] opcDaItemResults = As<OpcItemMgt>().AddItems(pItemArray, out ppErrors);

            OpcDaItemResult[] results = CreateItemResults(itemDefinitions, pItemArray, opcDaItemResults, ppErrors, true);

            var addedItems = new List<OpcDaItem>(itemDefinitions.Count);
            foreach (OpcDaItemResult result in results)
            {
                OpcDaItem item = result.Item;
                if (result.Error.Succeeded && item != null)
                {
                    _items.Add(item);
                    addedItems.Add(item);
                }
            }

            // Set client handles to index of items
            UpdateClientHandles();

            OnItemsChanged(new OpcDaItemsChangedEventArgs(addedItems.ToArray(), null, null));
            return results;
        }

        /// <summary>
        ///     Determines if an item is valid (could it be added without error). Also returns information about the item such as
        ///     canonical datatype. Does not affect the group in any way.
        /// </summary>
        /// <param name="itemDefinitions">The list of item definitions.</param>
        /// <param name="blobUpdate">
        ///     If set to <c>true</c> (and the server supports Blobs) the server should return updated Blobs
        ///     in results, otherwise the server will not return Blobs.
        /// </param>
        /// <returns>
        ///     Array of item results.
        /// </returns>
        public OpcDaItemResult[] ValidateItems(IList<OpcDaItemDefinition> itemDefinitions, bool blobUpdate = false)
        {
            OPCITEMDEF[] pItemArray = ArrayHelpers.CreateOPITEMDEFs(itemDefinitions);
            HRESULT[] ppErrors;
            OPCITEMRESULT[] opcDaItemResults = As<OpcItemMgt>().ValidateItems(pItemArray, blobUpdate, out ppErrors);
            OpcDaItemResult[] results = CreateItemResults(itemDefinitions, pItemArray, opcDaItemResults, ppErrors, false);
            return results;
        }

        /// <summary>
        ///     Removes (deletes) items from the group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <returns>
        ///     Array of HRESULTs. Indicates which items were successfully removed.
        /// </returns>
        public HRESULT[] RemoveItems(IList<OpcDaItem> items)
        {
            CheckItems(items);
            int[] serverHandles = ArrayHelpers.GetServerHandles(items);
            HRESULT[] ppErrors = As<OpcItemMgt>().RemoveItems(serverHandles);
            foreach (int serverHandle in serverHandles)
            {
                _items.RemoveAll(i => i.ServerHandle == serverHandle);
            }
            // Set client handles to index of items
            UpdateClientHandles();
            OnItemsChanged(new OpcDaItemsChangedEventArgs(null, items.ToArray(), null));
            return ppErrors;
        }

        /// <summary>
        ///     Sets one or more items in a group to active or inactive. This controls whether or not valid data can be obtained
        ///     from Read CACHE for those items and whether or not they are included in the OnDataChange subscription to the group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="isActive">If set to <c>true</c> items are to be activated, otherwise items are to be deactivated.</param>
        /// <returns>
        ///     Array of HRESULTs. Indicates which items were successfully affected.
        /// </returns>
        public HRESULT[] SetActiveItems(IList<OpcDaItem> items, bool isActive = true)
        {
            CheckItems(items);
            int[] serverHandles = ArrayHelpers.GetServerHandles(items);
            HRESULT[] ppErrors = As<OpcItemMgt>().SetActiveState(serverHandles, isActive);
            var changedItems = new List<OpcDaItem>(items.Count);
            for (int index = 0; index < items.Count; index++)
            {
                if (ppErrors[index].Succeeded)
                {
                    OpcDaItem item = items[index];
                    item.IsActive = isActive;
                    changedItems.Add(item);
                }
            }
            OnItemsChanged(new OpcDaItemsChangedEventArgs(null, null, changedItems.ToArray()));
            return ppErrors;
        }

        /// <summary>
        ///     Synchronizes the group items. It means existing items will be replaced with new items. It fires ItemsChanged event
        ///     after synchronization.
        /// </summary>
        public void SyncItems()
        {
            IEnumOPCItemAttributes enumerator = As<OpcItemMgt>().CreateEnumerator();
            List<OPCITEMATTRIBUTES> itemAttributes = enumerator.EnumareateAllAndRelease(OpcConfiguration.BatchSize);

            OpcDaItem[] oldItems = _items.ToArray();
            _items.Clear();
            foreach (OPCITEMATTRIBUTES opcitemattributes in itemAttributes)
            {
                var item = new OpcDaItem(opcitemattributes, this);
                _items.Add(item);
            }
            OnItemsChanged(new OpcDaItemsChangedEventArgs(_items.ToArray(), oldItems, null));
        }

        /// <summary>
        ///     Changes the requested data type for one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="requestedTypes">Array of new Requested Datatypes to be stored.</param>
        /// <returns>
        ///     Array of HRESULTs. Indicates which items were successfully affected.
        /// </returns>
        /// <exception cref="System.ArgumentException">Wrong size of requested types array.;requestedTypes</exception>
        public HRESULT[] SetDataTypes(IList<OpcDaItem> items, IList<Type> requestedTypes)
        {
            CheckItems(items);
            if (items.Count != requestedTypes.Count)
                throw new ArgumentException("Wrong size of requested types array.", "requestedTypes");

            int[] serverHandles = ArrayHelpers.GetServerHandles(items);
            IList<VarEnum> requestedDatatypes = requestedTypes.Select(TypeConverter.ToVarEnum).ToArray();
            HRESULT[] ppErrors = As<OpcItemMgt>().SetDatatypes(serverHandles, requestedDatatypes);

            var changedItems = new List<OpcDaItem>(items.Count);
            for (int index = 0; index < items.Count; index++)
            {
                if (ppErrors[index].Succeeded)
                {
                    OpcDaItem item = items[index];
                    item.RequestedDataType = requestedTypes[index];
                    changedItems.Add(item);
                }
            }

            OnItemsChanged(new OpcDaItemsChangedEventArgs(null, null, changedItems.ToArray()));
            return ppErrors;
        }

        /// <summary>
        ///     Changes the requested data type for one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="requestedType">New Requested Datatype for all items to be stored.</param>
        /// <returns>
        ///     Array of HRESULTs. Indicates which items were successfully affected.
        /// </returns>
        public HRESULT[] SetDataTypes(IList<OpcDaItem> items, Type requestedType = null)
        {
            var types = new Type[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                types[i] = requestedType;
            }
            return SetDataTypes(items, types);
        }

        /// <summary>
        ///     Creates a second copy of the group with a unique name.
        /// </summary>
        /// <param name="name">The name of the group. The name must be unique among the other groups created by this client.</param>
        /// <returns>
        ///     The group clone.
        /// </returns>
        public OpcDaGroup Clone(string name)
        {
            object comGroup = As<OpcGroupStateMgt>().CloneGroup(name);
            OpcDaGroup group = Server.CreateGroupWrapper(comGroup);
            group.SyncItems();
            return group;
        }

        /// <summary>
        /// Synchronizes the group state.
        /// </summary>
        public void SyncState()
        {
            int localeId;
            As<OpcGroupStateMgt>().GetState(out _updateRate, out _isActive, out _name, out _timeBias,
                out _percentDeadband, out localeId, out _clientHandle, out _serverHandle);
            _culture = CultureHelper.GetCultureInfo(localeId);

            _keepAlive = RefreshKeepAlive();
        }

        /// <summary>
        /// Sets the group state.
        /// </summary>
        /// <param name="state">the group state.</param>
        public void SetState(OpcDaGroupState state)
        {
            int localeId = CultureHelper.GetLocaleId(state.Culture);
            TimeSpan updateRate = As<OpcGroupStateMgt>().SetState(state.UpdateRate, state.IsActive, state.TimeBias,
                state.PercentDeadband, localeId, state.ClientHandle);

            if (state.UpdateRate.HasValue)
            {
                _updateRate = updateRate;
            }

            if (state.IsActive.HasValue)
            {
                _isActive = state.IsActive.Value;
            }

            if (state.TimeBias.HasValue)
            {
                _timeBias = state.TimeBias.Value;
            }

            if (state.PercentDeadband.HasValue)
            {
                _percentDeadband = state.PercentDeadband.Value;
            }

            if (state.Culture != null)
            {
                _culture = state.Culture;
            }

            if (state.ClientHandle.HasValue)
            {
                _clientHandle = state.ClientHandle.Value;
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
                return (T) Activator.CreateInstance(typeof (T), ComObject, Server);
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

        private OpcDaItemResult[] CreateItemResults(IList<OpcDaItemDefinition> itemDefinitions, OPCITEMDEF[] pItemArray,
            OPCITEMRESULT[] opcDaItemResults, HRESULT[] ppErrors, bool setGroup)
        {
            var results = new OpcDaItemResult[pItemArray.Length];
            for (int index = 0; index < opcDaItemResults.Length; index++)
            {
                OPCITEMRESULT opcItemResult = opcDaItemResults[index];
                OpcDaItemDefinition opcItemDefinition = itemDefinitions[index];
                HRESULT error = ppErrors[index];

                if (error.Succeeded)
                {
                    var item = new OpcDaItem(opcItemDefinition, opcItemResult, setGroup ? this : null)
                    {
                        UserData = opcItemDefinition.UserData
                    };
                    results[index] = new OpcDaItemResult(item, error);
                }
                else
                {
                    results[index] = new OpcDaItemResult(null, error);
                }
            }
            return results;
        }

        private void CheckSupported(OpcDaGroupFeatures feature)
        {
            switch (feature)
            {
                case OpcDaGroupFeatures.Read:
                case OpcDaGroupFeatures.Write:
                    if (!Is<OpcSyncIO>())
                        throw ExceptionHelper.NotSupportedDa2x();
                    break;
                case OpcDaGroupFeatures.ReadAsync:
                case OpcDaGroupFeatures.WriteAsync:
                case OpcDaGroupFeatures.RefreshAsync:
                case OpcDaGroupFeatures.Subscription:
                    if (!Is<OpcAsyncIO2>())
                        throw ExceptionHelper.NotSupportedDa2x();
                    break;
                case OpcDaGroupFeatures.ReadMaxAge:
                case OpcDaGroupFeatures.WriteVQT:
                    if (!Is<OpcSyncIO>())
                        throw ExceptionHelper.NotSupportedDa3x();
                    break;
                case OpcDaGroupFeatures.ReadMaxAgeAsync:
                case OpcDaGroupFeatures.RefreshMaxAgeAsync:
                case OpcDaGroupFeatures.WriteVQTAsync:
                    if (!Is<OpcAsyncIO3>())
                        throw ExceptionHelper.NotSupportedDa3x();
                    break;
                case OpcDaGroupFeatures.KeepAlive:
                    if (!Is<OpcItemMgt>())
                        throw ExceptionHelper.NotSupportedDa3x();
                    break;
                case OpcDaGroupFeatures.Sampling:
                    if (!Is<OpcItemSamplingMgt>())
                        throw ExceptionHelper.NotSupportedDa3x();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("feature", feature, null);
            }
        }

        private void CheckItems(IList<OpcDaItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
#if DEBUG
            foreach (OpcDaItem item in items)
            {
                if (item == null)
                    throw new ArgumentNullException("items");
                if (item.Group != this && _items.Contains(item))
                    throw new ArgumentException(string.Format("Item '{0}' doesn't belong to the group '{1}'.",
                        item.ItemId, Name));
            }
#endif
        }

        internal OpcDaItem GetItem(int clientHandle)
        {
            if (clientHandle < _items.Count)
            {
                OpcDaItem opcDaItem = _items[clientHandle];
                if (opcDaItem.ClientHandle == clientHandle)
                    return opcDaItem;
            }

            return _items.SingleOrDefault(i => i.ClientHandle == clientHandle);
        }

        private void UpdateClientHandles()
        {
            if (_items.Count == 0) // nothing to update
                return;

            var serverHandles = new int[_items.Count];
            var clientHandles = new int[_items.Count];
            for (int i = 0; i < _items.Count; i++)
            {
                OpcDaItem item = _items[i];
                serverHandles[i] = item.ServerHandle;
                clientHandles[i] = i;
            }

            HRESULT[] errors = SetClientHandles(serverHandles, clientHandles);

            for (int i = 0; i < errors.Length; i++)
            {
                if (errors[i].Succeeded) // Update only for succeded
                {
                    _items[i].ClientHandle = i;
                }
            }
        }

        private HRESULT[] SetClientHandles(int[] serverHandles, int[] clientHandles)
        {
            if (serverHandles.Length != clientHandles.Length)
                throw new ArgumentException("Wrong size of client handles array.", "clientHandles");
            HRESULT[] ppErrors = As<OpcItemMgt>().SetClientHandles(serverHandles, clientHandles);
            return ppErrors;
        }

        private TimeSpan RefreshKeepAlive()
        {
            if (!Is<OpcGroupStateMgt2>())
                return TimeSpan.Zero;
            return As<OpcGroupStateMgt2>().GetKeepAlive();
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

            ReleaseCom();
            // Free any unmanaged objects here.
            //
            _disposed = true;
        }

        private void ReleaseCom()
        {
            if (_asyncRequestManager != null)
            {
                _asyncRequestManager.Dispose();
                _asyncRequestManager = null;
            }

            try
            {
                if (ComObject != null)
                {
                    ComObject.ReleaseComServer();
                    ComObject = null;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Failed to release COM object of '{0}' group.", ex, Name);
            }

            try
            {
                OnDestroyed();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Destroy event of group '{0}' throws exception .", ex, Name);
            }
        }

        ~OpcDaGroup()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Raises the <see cref="E:ValuesChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="OpcDaItemValuesChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnValuesChanged(OpcDaItemValuesChangedEventArgs e)
        {
            EventHandler<OpcDaItemValuesChangedEventArgs> handler = ValuesChanged;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        ///     Called when the group has destroyed.
        /// </summary>
        protected virtual void OnDestroyed()
        {
            EventHandler handler = Destroyed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the <see cref="E:ItemsChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="OpcDaItemsChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnItemsChanged(OpcDaItemsChangedEventArgs e)
        {
            EventHandler<OpcDaItemsChangedEventArgs> handler = ItemsChanged;
            if (handler != null) handler(this, e);
        }
    }
}