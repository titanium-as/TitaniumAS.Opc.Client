using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace TitaniumAS.Opc.Client.Tests.Da.Browsing
{
    [TestClass]
    public class OpcDaBrowser1Tests
    {
        private OpcDaBrowser1 _opcBrowser;
        private OpcDaServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            var uri = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            _server = new OpcDaServer(uri);
            _server.Connect();
            _opcBrowser = new OpcDaBrowser1(_server);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void Test_Browser_TraverseTree()
        {
            BrowseChildren("", OpcDaBrowseDirection.To);
        }

        [TestMethod]
        public void Test_GetElements_AllProperties()
        {
            var elements = _opcBrowser.GetElements("", null, new OpcDaPropertiesQuery());
        }

        [TestMethod]
        public void Test_GetElements_Property()
        {
            OpcDaBrowseElement[] elements = _opcBrowser.GetElements("", null, new OpcDaPropertiesQuery(true, OpcDaItemPropertyIds.OPC_PROP_VALUE));
            elements.Should().NotBeEmpty();
            elements.Should().Contain(e => e.ItemProperties.Properties.Length == 1)
                .And.Contain(e=>e.IsItem)
                .And.Contain(e=>e.HasChildren);
        }

        [TestMethod]
        public void Test_GetElements_Branches()
        {
            _opcBrowser.GetElements(""); // Expand root
            OpcDaBrowseElement[] elements = _opcBrowser.GetElements("Simulation Items¥", new OpcDaElementFilter() { ElementType = OpcDaBrowseFilter.Branches }, new OpcDaPropertiesQuery(true, OpcDaItemPropertyIds.OPC_PROP_VALUE));
            elements.Should().NotBeEmpty()
                .And.OnlyContain(e => e.HasChildren);
        }

        [TestMethod]
        public void Test_GetElements_Leafs()
        {
            OpcDaBrowseElement[] elements = _opcBrowser.GetElements(""); // Expand root
            foreach (var opcDaBrowseElement in elements)
            {
                if (opcDaBrowseElement.HasChildren)
                {
                    _opcBrowser.GetElements(opcDaBrowseElement.ItemId);
                }
            }
            
            elements = _opcBrowser.GetElements("Random", new OpcDaElementFilter() { ElementType = OpcDaBrowseFilter.Items }, new OpcDaPropertiesQuery(true, OpcDaItemPropertyIds.OPC_PROP_VALUE));
            elements.Should().NotBeEmpty()
                .And.OnlyContain(e => e.IsItem);
        }

        [TestMethod] public void Test_Browser_TraverseTree_With_GetElement()
        {
            BrowseHelpers.BrowseChildren(null, _opcBrowser);
        }

        private void BrowseChildren(string itemId, OpcDaBrowseDirection browseDirection)
        {
            OpcDaBrowseElement[] branches = _opcBrowser.GetElements(itemId, new OpcDaElementFilter()
            {
                ElementType = OpcDaBrowseFilter.Branches
            });
            foreach (var branch in branches)
            {
                BrowseChildren(branch.ItemId, OpcDaBrowseDirection.Down);
            }

            var leafs = _opcBrowser.GetElements(itemId, new OpcDaElementFilter()
            {
                ElementType = OpcDaBrowseFilter.Items
            });
            foreach (var leaf in leafs)
            {
            }
        }
    }
}