using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA group interface.
    /// </summary>
    public interface IOpcDaGroup
    {
        /// <summary>
        /// Occurs when the group has destroyed.
        /// </summary>
        event EventHandler Destroyed;

        /// <summary>
        /// Occurs when the group items changed.
        /// </summary>
        event EventHandler<OpcDaItemsChangedEventArgs> ItemsChanged;

        /// <summary>
        /// Gets the items of the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        ReadOnlyCollection<OpcDaItem> Items { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the group is subscribed.
        /// </summary>
        /// <value>
        /// <c>true</c> if the group is subscribed; otherwise, <c>false</c>.
        /// </value>
        bool IsSubscribed { get; set; }

        /// <summary>
        /// Gets or sets the update rate requested for the group (milliseconds).
        /// </summary>
        /// <value>
        /// The update rate of the group (milliseconds).
        /// </value>
        TimeSpan UpdateRate { get; set; }

        /// <summary>
        /// Gets the server handle of the group.
        /// </summary>
        /// <value>
        /// The server handle of the group.
        /// </value>
        int ServerHandle { get; }

        /// <summary>
        /// Gets or sets the client handle of the group.
        /// </summary>
        /// <value>
        /// The client handle.
        /// </value>
        int ClientHandle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the group is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the group is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the TimeZone Bias of the group (in minutes).
        /// </summary>
        /// <value>
        /// The TimeZone Bias of the group (in minutes).
        /// </value>
        TimeSpan TimeBias { get; set; }

        /// <summary>
        /// Gets or sets the range of the Deadband is from 0.0 to 100.0 Percent.
        /// </summary>
        /// <value>
        /// The percent deadband.
        /// </value>
        float PercentDeadband { get; set; }

        /// <summary>
        /// Gets or sets the current culture for the group.
        /// </summary>
        /// <value>
        /// The current culture for the group.
        /// </value>
        CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum interval (in milliseconds) between subscription callbacks. A zero value indicates the client does not wish to receive any empty keep-alive callbacks.
        /// </summary>
        /// <value>
        /// The keep alive time (in milliseconds).
        /// </value>
        TimeSpan KeepAlive { get; set; }

        /// <summary>
        /// Gets the server to which the group belongs to.
        /// </summary>
        /// <value>
        /// The server to which the group belongs to.
        /// </value>
        OpcDaServer Server { get; }

        /// <summary>
        /// Gets or attachs arbitrary user data to the group.
        /// </summary>
        /// <value>
        /// The user data.
        /// </value>
        object UserData { get; set; }

        /// <summary>
        /// Gets the actual COM object.
        /// </summary>
        /// <value>
        /// The actual COM object.
        /// </value>
        object ComObject { get; }

        /// <summary>
        /// Determines whether the specified features are supported.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <returns><c>true</c> if the specified features are supported; otherwise, <c>false</c>.</returns>
        bool IsSupported(OpcDaGroupFeatures features);

        /// <summary>
        /// Occurs when the values of the group items changed.
        /// </summary>
        event EventHandler<OpcDaItemValuesChangedEventArgs> ValuesChanged;

        /// <summary>
        /// Reads the specified group items.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="dataSource">The data source.</param>
        /// <returns>The values of the group items.</returns>
        OpcDaItemValue[] Read(IList<OpcDaItem> items, OpcDaDataSource dataSource = OpcDaDataSource.Cache);

        /// <summary>
        /// Reads the specified group items asynchronously.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> ReadAsync(IList<OpcDaItem> items, CancellationToken token);

        /// <summary>
        /// Reads the specified group items asynchronously.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> ReadAsync(IList<OpcDaItem> items);

        /// <summary>
        /// Reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// //In addition, the ability to Read from a group based on a "MaxAge" is provided.
        /// //from cache or device as determined by "staleness" of cache data
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The MaxAge for all items.</param>
        /// <returns>The values of the group items.</returns>
        OpcDaItemValue[] ReadMaxAge(IList<OpcDaItem> items, TimeSpan maxAge);

        /// <summary>
        /// Reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The list of MaxAges for the group items.</param>
        /// <returns>The values of the group items.</returns>
        OpcDaItemValue[] ReadMaxAge(IList<OpcDaItem> items, IList<TimeSpan> maxAge);


        /// <summary>
        /// Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The MaxAge for all items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, TimeSpan maxAge,
            CancellationToken token);

        /// <summary>
        /// Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The MaxAge for all items.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, TimeSpan maxAge);

        /// <summary>
        /// Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The list of MaxAges for the group items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, IList<TimeSpan> maxAge,
            CancellationToken token);

        /// <summary>
        /// Asynchronously reads the specified group items using MaxAge. If the information in the cache is within the MaxAge, then the data will be obtained from the cache, otherwise the server must access the device for the requested information.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="maxAge">The list of MaxAges for the group items.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> ReadMaxAgeAsync(IList<OpcDaItem> items, IList<TimeSpan> maxAge);

        /// <summary>
        /// Asynchronously refreshes all active group items (whether they have changed or not). Inactive items are not included in the callback.
        /// </summary>
        /// <param name="dataSource">The data source either cache or device.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> RefreshAsync(OpcDaDataSource dataSource, CancellationToken token);

        /// <summary>
        /// Asynchronously refreshes all active group items (whether they have changed or not). Inactive items are not included in the callback.
        /// </summary>
        /// <param name="dataSource">The data source either cache or device.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> RefreshAsync(OpcDaDataSource dataSource);

        /// <summary>
        /// Asynchronously refreshes all active group items (whether they have changed or not) using MaxAge. Inactive items are not included in the callback. Some of the values may be obtained from cache while others could be obtained from the device depending on the "freshness" of the data in the cache.
        /// </summary>
        /// <param name="maxAge">The MaxAge value, which will determine the MaxAge for all active items in the group.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> RefreshMaxAgeAsync(TimeSpan maxAge, CancellationToken token);

        /// <summary>
        /// Asynchronously refreshes all active group items (whether they have changed or not) using MaxAge. Inactive items are not included in the callback. Some of the values may be obtained from cache while others could be obtained from the device depending on the "freshness" of the data in the cache.
        /// </summary>
        /// <param name="maxAge">The MaxAge value, which will determine the MaxAge for all active items in the group.</param>
        /// <returns>The values of the group items.</returns>
        Task<OpcDaItemValue[]> RefreshMaxAgeAsync(TimeSpan maxAge);

        /// <summary>
        /// Writes values to one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values to be written to the items.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        HRESULT[] Write(IList<OpcDaItem> items, object[] values);

        /// <summary>
        /// Asynchronously writes values to one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values to be written to the items.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        Task<HRESULT[]> WriteAsync(IList<OpcDaItem> items, IList<object> values,
            CancellationToken token);

        /// <summary>
        /// Asynchronously writes values to one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values to be written to the items.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        Task<HRESULT[]> WriteAsync(IList<OpcDaItem> items, IList<object> values);

        /// <summary>
        /// Writes one or more values, qualities and timestamps for the items specified. If a client attempts to write VQ, VT, or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        HRESULT[] WriteVQT(IList<OpcDaItem> items, IList<OpcDaVQT> values);

        /// <summary>
        /// Asynchronously writes one or more values, qualities and timestamps for the items specified. If a client attempts to write VQ, VT, or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <param name="token">The task cancellation token.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        Task<HRESULT[]> WriteVQTAsync(IList<OpcDaItem> items, IList<OpcDaVQT> values,
            CancellationToken token);

        /// <summary>
        /// Asynchronously writes one or more values, qualities and timestamps for the items specified. If a client attempts to write VQ, VT, or VQT it should expect that the server will write them all or none at all.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="values">The list of values, qualities and timestamps.</param>
        /// <returns>Array of HRESULTs indicating the success of the individual item Writes.</returns>
        Task<HRESULT[]> WriteVQTAsync(IList<OpcDaItem> items, IList<OpcDaVQT> values);

        /// <summary>
        /// Add one or more items to the group.
        /// </summary>
        /// <param name="itemDefinitions">The list of item definitions.</param>
        /// <returns>Array of item results.</returns>
        OpcDaItemResult[] AddItems(IList<OpcDaItemDefinition> itemDefinitions);

        /// <summary>
        /// Determines if an item is valid (could it be added without error). Also returns information about the item such as canonical datatype. Does not affect the group in any way.
        /// </summary>
        /// <param name="itemDefinitions">The list of item definitions.</param>
        /// <param name="blobUpdate">If set to <c>true</c> (and the server supports Blobs) the server should return updated Blobs in results, otherwise the server will not return Blobs.</param>
        /// <returns>Array of item results.</returns>
        OpcDaItemResult[] ValidateItems(IList<OpcDaItemDefinition> itemDefinitions, bool blobUpdate = false);

        /// <summary>
        /// Removes (deletes) items from the group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <returns>Array of HRESULTs. Indicates which items were successfully removed.</returns>
        HRESULT[] RemoveItems(IList<OpcDaItem> items);

        /// <summary>
        /// Sets one or more items in a group to active or inactive. This controls whether or not valid data can be obtained from Read CACHE for those items and whether or not they are included in the OnDataChange subscription to the group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="isActive">If set to <c>true</c> items are to be activated, otherwise items are to be deactivated.</param>
        /// <returns>Array of HRESULTs. Indicates which items were successfully affected.</returns>
        HRESULT[] SetActiveItems(IList<OpcDaItem> items, bool isActive = true);

        /// <summary>
        /// Synchronizes the group items. It means existing items will be replaced with new items. It fires ItemsChanged event after synchronization.
        /// </summary>
        void SyncItems();

        /// <summary>
        /// Changes the requested data type for one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="requestedTypes">Array of new Requested Datatypes to be stored.</param>
        /// <returns>Array of HRESULTs. Indicates which items were successfully affected.</returns>
        HRESULT[] SetDataTypes(IList<OpcDaItem> items, IList<Type> requestedTypes);

        /// <summary>
        /// Changes the requested data type for one or more items in a group.
        /// </summary>
        /// <param name="items">The group items.</param>
        /// <param name="requestedType">New Requested Datatype for all items to be stored.</param>
        /// <returns>
        /// Array of HRESULTs. Indicates which items were successfully affected.
        /// </returns>
        HRESULT[] SetDataTypes(IList<OpcDaItem> items, Type requestedType = null);

        /// <summary>
        /// Creates a second copy of the group with a unique name.
        /// </summary>
        /// <param name="name">The name of the group. The name must be unique among the other groups created by this client.</param>
        /// <returns>The group clone.</returns>
        OpcDaGroup Clone(string name);

        /// <summary>
        /// Synchronizes the group state.
        /// </summary>
        void SyncState();

        /// <summary>
        /// Sets the group state.
        /// </summary>
        /// <param name="state">the group state.</param>
        void SetState(OpcDaGroupState state);
    }
}