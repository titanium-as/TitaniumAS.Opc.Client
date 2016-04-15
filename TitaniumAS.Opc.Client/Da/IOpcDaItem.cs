using System;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA item interface.
    /// </summary>
    public interface IOpcDaItem
    {
        /// <summary>
        /// Occurs when the item changed.
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Gets the group to which the item belongs to.
        /// </summary>
        /// <value>
        /// The group to which the item belongs to.
        /// </value>
        OpcDaGroup Group { get; }

        /// <summary>
        /// The handle the client has associated with the item.
        /// </summary>
        /// <value>
        /// The client handle for the item.
        /// </value>
        int ClientHandle { get; }

        /// <summary>
        /// Gets the handle the server uses to reference the item.
        /// </summary>
        /// <value>
        /// The server handle for the item.
        /// </value>
        int ServerHandle { get; }

        /// <summary>
        /// Gets the unique identifier for the item.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        string ItemId { get; }

        /// <summary>
        /// Gets the data type of item value. If the requested data type was rejected the canonical data type will be returned.
        /// </summary>
        /// <value>
        /// The requested data type.
        /// </value>
        Type RequestedDataType { get; }

        /// <summary>
        /// Gets the data type of item value is maintained within the server. A server that does not know the canonical type of an item (e.g. during startup) should return VT_EMPTY. This is an indication to the client that the datatype is not currently known but will be determined at some future time.
        /// </summary>
        /// <value>
        /// The canonical data type.
        /// </value>
        Type CanonicalDataType { get; }

        /// <summary>
        /// Gets the Blob associated with the item.
        /// </summary>
        /// <value>
        /// The Blob.
        /// </value>
        byte[] Blob { get; }

        /// <summary>
        /// Gets the access path the server should associate with the item. A null object specifies that the server should select the access path. It is optional for support.
        /// </summary>
        /// <value>
        /// The access path.
        /// </value>
        string AccessPath { get; }

        /// <summary>
        /// Gets a value indicating whether the item is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the item is active; otherwise, <c>false</c>.
        /// </value>
        bool IsActive { get; }

        /// <summary>
        /// Gets the access rights indicates if the item is read only, write only or read/write. This is not related to security but rather to the nature of the underlying hardware.
        /// </summary>
        /// <value>
        /// The access rights.
        /// </value>
        OpcDaAccessRights AccessRights { get; }

        /// <summary>
        /// Gets or attachs arbitrary user data to the item.
        /// </summary>
        /// <value>
        /// The user data.
        /// </value>
        object UserData { get; set; }
    }
}