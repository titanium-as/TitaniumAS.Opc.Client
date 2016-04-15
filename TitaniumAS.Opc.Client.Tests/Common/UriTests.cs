using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TitaniumAS.Opc.Client.Tests.Common
{
    [TestClass]
    public class UriTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Test_Build()
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = "opcda",
                Host = "localhost",
                Path = "Matrikon.OPC.Simulation.1"
            };
            var uri = uriBuilder.Uri;
            uri.Should().Be("opcda://localhost/Matrikon.OPC.Simulation.1");
        }
     }
}