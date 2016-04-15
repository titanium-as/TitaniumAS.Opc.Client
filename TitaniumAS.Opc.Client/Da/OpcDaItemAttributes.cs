using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents OPC DA item attributes.
    /// </summary>
    public class OpcDaItemAttributes
    {
        internal OpcDaItemAttributes(OPCITEMATTRIBUTES itemAttributes)
        {
            AccessPath = itemAttributes.szAccessPath;
            ItemID = itemAttributes.szItemID;
            IsActive = itemAttributes.bActive;
            ClientHandle = itemAttributes.hClient;
            ServerHandle = itemAttributes.hServer;
            AccessRights = (OpcDaAccessRights) itemAttributes.dwAccessRights;
            Blob = new byte[itemAttributes.dwBlobSize];
            Marshal.Copy(itemAttributes.pBlob, Blob, 0, Blob.Length);
            RequestedDataType = TypeConverter.FromVarEnum((VarEnum) itemAttributes.vtRequestedDataType);
            CanonicalDataType = TypeConverter.FromVarEnum((VarEnum) itemAttributes.vtCanonicalDataType);
            EUType = (OpcDaEUType) itemAttributes.dwEUType;
            EUInfo = itemAttributes.vEUInfo;
        }

        /// <summary>
        /// Gets or sets the access path specified by the client. A null value is returned if the server does not support access paths.
        /// </summary>
        /// <value>
        /// The access path.
        /// </value>
        public string AccessPath { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the item.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public string ItemID { get; set; }

        /// <summary>
        /// Gets a value indicating whether the item is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the item is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// The handle the client has associated with the item.
        /// </summary>
        /// <value>
        /// The client handle for the item.
        /// </value>
        public int ClientHandle { get; set; }

        /// <summary>
        /// Gets or sets the handle the server uses to reference the item.
        /// </summary>
        /// <value>
        /// The server handle for the item.
        /// </value>
        public int ServerHandle { get; set; }

        /// <summary>
        /// Gets or sets the access rights indicates if the item is read only, write only or read/write. This is not related to security but rather to the nature of the underlying hardware.
        /// </summary>
        /// <value>
        /// The access rights.
        /// </value>
        public OpcDaAccessRights AccessRights { get; set; }

        /// <summary>
        /// Gets or sets the Blob associated with the item.
        /// </summary>
        /// <value>
        /// The Blob.
        /// </value>
        public byte[] Blob { get; set; }

        /// <summary>
        /// Gets the data type of item value. If the requested data type was rejected the canonical data type will be returned.
        /// </summary>
        /// <value>
        /// The requested data type.
        /// </value>
        public Type RequestedDataType { get; set; }

        /// <summary>
        /// Gets the data type of item value is maintained within the server. A server that does not know the canonical type of an item (e.g. during startup) should return VT_EMPTY. This is an indication to the client that the datatype is not currently known but will be determined at some future time.
        /// </summary>
        /// <value>
        /// The canonical data type.
        /// </value>
        public Type CanonicalDataType { get; set; }

        /// <summary>
        /// Gets or sets the type of Engineering Units (EU) information (if any) contained in the EU information.
        /// </summary>
        /// <value>
        /// The type of Engineering Units (EU) information.
        /// </value>
        public OpcDaEUType EUType { get; set; }

        /// <summary>
        /// Gets or sets object containing the Engineering Units (EU) information. The EU support is optional. 
        /// </summary>
        /// <value>
        /// The Engineering Units (EU) information.
        /// </value>
        public object EUInfo { get; set; }
    }
}