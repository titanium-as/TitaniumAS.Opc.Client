using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace TitaniumAS.Opc.Client.Tests.Da.Browsing
{
    [TestClass]
    public class OpcDaBrowser3Tests
    {
        private OpcDaServer _graybox;
        private OpcDaBrowser3 _grayboxBrowser;
        private OpcDaServer _matrikon;
        private OpcDaBrowser3 _matrikonBrowser;

        [TestInitialize]
        public void TestInitialize()
        {
            _matrikon = new OpcDaServer(UrlBuilder.Build("Matrikon.OPC.Simulation.1"));
            _matrikon.Connect();
            _graybox = new OpcDaServer(UrlBuilder.Build("Graybox.Simulator.1"));
            _graybox.Connect();
            _matrikonBrowser = new OpcDaBrowser3(_matrikon);
            _grayboxBrowser = new OpcDaBrowser3(_graybox);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _graybox.Dispose();
            _matrikon.Dispose();
        }

        [TestMethod]
        public void Test_Browser_Filter_Branches()
        {
            var opcElements = _matrikonBrowser.GetElements("",
                new OpcDaElementFilter {ElementType = OpcDaBrowseFilter.Branches});
            opcElements.Should().HaveCount(2).And.OnlyContain(e => e.HasChildren && !e.IsItem);
        }

        [TestMethod]
        public void Test_Browser_Filter_Items()
        {
            var opcElements = _matrikonBrowser.GetElements("",
                new OpcDaElementFilter {ElementType = OpcDaBrowseFilter.Items});
            opcElements.Should().HaveCount(2).And.OnlyContain(e => !e.HasChildren && e.IsItem);
        }

        [TestMethod]
        public void Test_Browser_Filter_All()
        {
            var opcElements = _matrikonBrowser.GetElements("");
            opcElements.Should().HaveCount(4);
        }

        [TestMethod]
        public void Test_Browser_Filter_Name()
        {
            var opcElements = _matrikonBrowser.GetElements("", new OpcDaElementFilter
            {
                ElementType = OpcDaBrowseFilter.All,
                Name = "*Cli*"
            });
            opcElements.Should().HaveCount(1);
        }

        [TestMethod]
        public void Test_Browser_TraverseTree_Matrikon()
        {
            BrowseHelpers.BrowseChildren("", _matrikonBrowser);
        }

        [TestMethod]
        public void Test_Browser_GetProperties()
        {
            var properties = _matrikonBrowser.GetProperties(new[] {"Random.Int1", "Random.Int2"},
                new OpcDaPropertiesQuery(true, 1, 2, 3, 4, 5, 6));
            foreach (var itemProperties in properties)
            {
                itemProperties.ErrorId.Should().Match<HRESULT>(e => e.Succeeded);
                itemProperties.Properties.Should().HaveCount(6);
            }
        }

        [TestMethod]
        public void Test_Browser_GetProperties_All()
        {
            var properties = _grayboxBrowser.GetProperties(
                new[] {"numeric.random.int32", "numeric.random.int16"},
                new OpcDaPropertiesQuery(true));
            foreach (var itemProperties in properties)
            {
                itemProperties.ErrorId.Should().Match<HRESULT>(e => e.Succeeded);
                itemProperties.Properties.Should().NotBeEmpty().And.OnlyContain(p => p.Value != null);
            }
        }

        [TestMethod]
        public void Test_Browser_GetProperties_NoValue()
        {
            var properties = _grayboxBrowser.GetProperties(new[] {"numeric.random.int32", "numeric.random.int16"},
                new OpcDaPropertiesQuery(false));
            foreach (var itemProperties in properties)
            {
                itemProperties.Properties.Should().NotBeEmpty().And.OnlyContain(p => p.Value == null);
            }
        }

        [TestMethod]
        public void Test_Browser_GetElements_WithProperties()
        {
            var elements = _grayboxBrowser.GetElements("numeric.random", null, new OpcDaPropertiesQuery());
            foreach (var element in elements)
            {
                element.ItemProperties.Should().Match<OpcDaItemProperties>(p => p.Properties.Length > 0);
            }
        }

        [TestMethod]
        public void Test_Browser_TraverseTree_Graybox()
        {
            BrowseHelpers.BrowseChildren("", _grayboxBrowser);
        }

        [TestMethod]
        public void Test_GetProperties()
        {
            var properties = _grayboxBrowser.GetProperties(new[] {"numeric.random.int32"});
            properties[0].Properties.Should().NotBeEmpty();
        }
    }
}