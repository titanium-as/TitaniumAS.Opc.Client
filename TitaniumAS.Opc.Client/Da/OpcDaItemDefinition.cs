using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Da
{
    /// <summary>
    /// Represents OPC DA item definition.
    /// </summary>
    public class OpcDaItemDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaItemDefinition"/> class.
        /// </summary>
        public OpcDaItemDefinition()
        {
            IsActive = true;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the item.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets of sets a value indicating whether the item is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the item is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the access path the server should associate with the item. A null object specifies that the server should select the access path. It is optional for support.
        /// </summary>
        /// <value>
        /// The access path.
        /// </value>
        public string AccessPath { get; set; }

        /// <summary>
        /// Gets or sets the Blob associated with the item.
        /// </summary>
        /// <value>
        /// The Blob.
        /// </value>
        public byte[] Blob { get; set; }

        /// <summary>
        /// Gets or sets the data type of item value. If the requested data type was rejected the canonical data type will be returned. Passing null means the client will accept the server's canonical datatype.
        /// </summary>
        /// <value>
        /// The requested data type.
        /// </value>
        public Type RequestedDataType { get; set; }

        /// <summary>
        /// Gets or attachs arbitrary user data to the item.
        /// </summary>
        /// <value>
        /// The user data.
        /// </value>
        public object UserData { get; set; }

        internal OPCITEMDEF ToStruct()
        {
            var itemDef = new OPCITEMDEF();
            itemDef.szItemID = ItemId;
            itemDef.bActive = IsActive ? 1 : 0;
            itemDef.hClient = 0;
            itemDef.szAccessPath = AccessPath;
            itemDef.vtRequestedDataType = (short) TypeConverter.ToVarEnum(RequestedDataType);
            if (Blob != null)
            {
                itemDef.dwBlobSize = Blob.Length;
                itemDef.pBlob = Marshal.AllocCoTaskMem(Blob.Length*Marshal.SizeOf(typeof (byte)));
                Marshal.Copy(Blob, 0, itemDef.pBlob, Blob.Length);
            }
            else
            {
                itemDef.dwBlobSize = 0;
                itemDef.pBlob = IntPtr.Zero;
            }
            return itemDef;
        }
    }
}