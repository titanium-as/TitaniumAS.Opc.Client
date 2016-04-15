using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents the OPC DA item.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.IOpcDaItem" />
    public class OpcDaItem : IOpcDaItem
    {
        private bool _isActive;
        private Type _requestedDataType;
        private int _clientHandle;

        internal OpcDaItem(OpcDaItemDefinition itemDefinition, OPCITEMRESULT itemResult, OpcDaGroup @group)
        {
            Group = @group;
            ClientHandle = 0;
            ServerHandle = itemResult.hServer;
            ItemId = itemDefinition.ItemId;
            RequestedDataType = itemDefinition.RequestedDataType;
            CanonicalDataType = TypeConverter.FromVarEnum((VarEnum) itemResult.vtCanonicalDataType);
            Blob = itemResult.pBlob;
            AccessPath = itemDefinition.AccessPath;
            IsActive = itemDefinition.IsActive;
            AccessRights = (OpcDaAccessRights) itemResult.dwAccessRights;
        }

        internal OpcDaItem(OPCITEMATTRIBUTES opcItemDefinition, OpcDaGroup @group)
        {
            Group = @group;
            ClientHandle = opcItemDefinition.hClient;
            ServerHandle = opcItemDefinition.hServer;
            ItemId = opcItemDefinition.szItemID;
            RequestedDataType = TypeConverter.FromVarEnum((VarEnum) opcItemDefinition.vtRequestedDataType);
            CanonicalDataType = TypeConverter.FromVarEnum((VarEnum) opcItemDefinition.vtCanonicalDataType);
            AccessPath = opcItemDefinition.szAccessPath;
            IsActive = opcItemDefinition.bActive;
            AccessRights = (OpcDaAccessRights) opcItemDefinition.dwAccessRights;
            if (opcItemDefinition.pBlob != IntPtr.Zero)
            {
                Blob = new byte[opcItemDefinition.dwBlobSize];
                Marshal.Copy(opcItemDefinition.pBlob, Blob, 0, Blob.Length);
            }
        }

        /// <summary>
        /// Occurs when the item changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Gets the group to which the item belongs to.
        /// </summary>
        /// <value>
        /// The group to which the item belongs to.
        /// </value>
        public OpcDaGroup Group { get; internal set; }

        /// <summary>
        /// The handle the client has associated with the item.
        /// </summary>
        /// <value>
        /// The client handle for the item.
        /// </value>
        public int ClientHandle
        {
            get { return _clientHandle; }
            internal set
            {
                if (_clientHandle == value) return;
                _clientHandle = value;
                OnChanged();
            }
        }

        /// <summary>
        /// Gets the handle the server uses to reference the item.
        /// </summary>
        /// <value>
        /// The server handle for the item.
        /// </value>
        public int ServerHandle { get; internal set; }

        /// <summary>
        /// Gets the unique identifier for the item.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public string ItemId { get; internal set; }

        /// <summary>
        /// Gets the data type of item value. If the requested data type was rejected the canonical data type will be returned.
        /// </summary>
        /// <value>
        /// The requested data type.
        /// </value>
        public Type RequestedDataType
        {
            get { return _requestedDataType; }
            internal set
            {
                if (_requestedDataType == value)
                    return;

                _requestedDataType = value;
                OnChanged();
            }
        }

        /// <summary>
        /// Gets the data type of item value is maintained within the server. A server that does not know the canonical type of an item (e.g. during startup) should return VT_EMPTY. This is an indication to the client that the datatype is not currently known but will be determined at some future time.
        /// </summary>
        /// <value>
        /// The canonical data type.
        /// </value>
        public Type CanonicalDataType { get; internal set; }

        /// <summary>
        /// Gets the Blob associated with the item.
        /// </summary>
        /// <value>
        /// The Blob.
        /// </value>
        public byte[] Blob { get; internal set; }

        /// <summary>
        /// Gets the access path the server should associate with the item. A null object specifies that the server should select the access path. It is optional for support.
        /// </summary>
        /// <value>
        /// The access path.
        /// </value>
        public string AccessPath { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the item is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the item is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get { return _isActive; }
            internal set
            {
                if (_isActive == value) return;
                _isActive = value;
                OnChanged();
            }
        }

        /// <summary>
        /// Gets the access rights indicates if the item is read only, write only or read/write. This is not related to security but rather to the nature of the underlying hardware.
        /// </summary>
        /// <value>
        /// The access rights.
        /// </value>
        public OpcDaAccessRights AccessRights { get; internal set; }

        /// <summary>
        /// Gets or attachs arbitrary user data to the item.
        /// </summary>
        /// <value>
        /// The user data.
        /// </value>
        public object UserData { get; set; }

        /// <summary>
        /// Called when the item changed.
        /// </summary>
        protected virtual void OnChanged()
        {
            var handler = Changed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}