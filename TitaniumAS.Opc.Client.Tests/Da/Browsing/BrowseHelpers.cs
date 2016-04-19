using System.Diagnostics;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace TitaniumAS.Opc.Client.Tests.Da.Browsing
{
    static internal class BrowseHelpers
    {
        public static void BrowseChildren(string itemId, IOpcDaBrowser browser)
        {
            var opcElements = browser.GetElements(itemId);

            foreach (OpcDaBrowseElement opcBrowseElement in opcElements)
            {
                Debug.WriteLine(opcBrowseElement);
                foreach (OpcDaItemProperty opcDaItemProperty in opcBrowseElement.ItemProperties.Properties)
                {
                    Debug.Indent();
                    Debug.WriteLine(opcDaItemProperty);
                    Debug.Unindent();
                }

                if (!opcBrowseElement.HasChildren)
                {
                    continue;
                }

                Debug.Indent();
                BrowseChildren(opcBrowseElement.ItemId, browser);
                Debug.Unindent();
            }
        }
    }
}