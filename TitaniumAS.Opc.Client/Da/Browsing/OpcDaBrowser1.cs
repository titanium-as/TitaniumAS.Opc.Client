using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Da.Browsing
{
    /// <summary>
    /// Represents an OPC DA 1.0 browser.
    /// </summary>
    /// <seealso cref="TitaniumAS.Opc.Da.Browsing.OpcDaBrowser2" />
    public class OpcDaBrowser1 : OpcDaBrowser2
    {
        readonly Dictionary<string, IEnumerable<string>> _itemIdToPath = new Dictionary<string, IEnumerable<string>>();
        private IEnumerable<string> _currentPath;

        /// <summary>
        /// The maximum depth of elements tree.
        /// </summary>
        public static int MaxDepth = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpcDaBrowser1"/> class.
        /// </summary>
        /// <param name="opcDaServer">The OPC DA server for browsing.</param>
        public OpcDaBrowser1(OpcDaServer opcDaServer = null) : base(opcDaServer)
        {
            _currentPath = new string[0];
            _itemIdToPath[String.Empty] = _currentPath;
        }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <param name="parentItemId">The parent item identifier.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="propertiesQuery">The properties query.</param>
        /// <returns></returns>
        public override OpcDaBrowseElement[] GetElements(string parentItemId, OpcDaElementFilter filter = null,
            OpcDaPropertiesQuery propertiesQuery = null)
        {
            OpcDaBrowseElement[] elements = base.GetElements(parentItemId, filter, propertiesQuery);
            SavePathes(elements);
            return elements;
        }

        private void SavePathes(OpcDaBrowseElement[] elements)
        {
            foreach (var element in elements)
            {
                IEnumerable<string> pathPart = Enumerable.Repeat(element.Name, 1);
                _itemIdToPath[element.ItemId] = _currentPath.Union(pathPart);
            }
        }

        protected override void ChangeBrowsePositionTo(string itemId)
        {
            IEnumerable<string> path;
            if (!_itemIdToPath.TryGetValue(itemId, out path))
            {
                throw new ArgumentException("Item not discovered yet. In OPC DA 1.X all parent branches should be expanded before expanding the element.", "itemId");
            }

            MoveToRoot();
            if (String.IsNullOrEmpty(itemId)) // Already at root
                return; 

            // Browse down using path
            foreach (var itemName in path)
            {
                OpcBrowseServerAddressSpace.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_DOWN, itemName);
            }

            _currentPath = path;
        }

        private void MoveToRoot()
        {
            try
            {
                for (int i = 0; i < MaxDepth; ++i)
                {
                    // will throw if we aleady at root
                    OpcBrowseServerAddressSpace.ChangeBrowsePosition(OPCBROWSEDIRECTION.OPC_BROWSE_UP);
                }
            }
            catch(COMException ex)
            {
                if (ex.ErrorCode == HRESULT.E_FAIL) // E_FAIL when change position on root element;
                    return;
                throw;
            }
        }
    }
}
