namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Represents a single element result of OPC DA server browsing.
    /// </summary>
    public class OpcDaBrowseElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaBrowseElement"/> class.
        /// </summary>
        public OpcDaBrowseElement()
        {
        }

        /// <summary>
        /// A user friendly part of the namespace pointing to the element. The name can be used for display purposes in a tree control.
        /// </summary>
        /// <value>
        /// A user friendly part of the namespace pointing to the element.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// A value indicating whether the element has children.
        /// </summary>
        /// <value>
        /// <c>true</c> if the element has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren { get; set; }

        /// <summary>
        /// A value indicating whether the element is item that can be used to Read, Write, and Subscribe.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the element is item; otherwise, <c>false</c>.
        /// </value>
        public bool IsItem { get; set; }

        /// <summary>
        /// The unique identifier of the item that can be used with AddItems, Browse or GetProperties.
        /// </summary>
        /// <value>
        /// The item identifier.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// The item properties of the element.
        /// </summary>
        /// <value>
        /// The item properties of the element.
        /// </value>
        public OpcDaItemProperties ItemProperties { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is hint versus being a valid item.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is hint; otherwise, <c>false</c>.
        /// </value>
        public bool IsHint
        {
            get { return IsItem && ItemId == null; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the element.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents the element.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} (ItemId: {1}, IsHint: {2}, IsItem: {3}, HasChildren: {4})", Name, ItemId, IsHint, IsItem, HasChildren);
        }
    }
}