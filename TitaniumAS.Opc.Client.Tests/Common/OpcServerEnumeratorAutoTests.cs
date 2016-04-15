using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Tests.Common
{
    [TestClass]
    public class OpcServerEnumeratorAutoTests
    {
        [TestCleanup]
        public void TestCleanup()
        {
            Marshal.AreComObjectsAvailableForCleanup().Should().BeFalse();
        }

        [TestMethod]
        public void Test_Enumerate_OpcDa10_Servers()
        {
            var enumerator = new OpcServerEnumeratorAuto();
            var serverDescriptions = enumerator.Enumerate(enumerator.Localhost, OpcServerCategory.OpcDaServer10);
            serverDescriptions.Should().ContainSingle(s => s.ProgId == "Matrikon.OPC.Simulation.1");
            serverDescriptions.Should().ContainSingle(s => s.Uri.Segments[1] == "Matrikon.OPC.Simulation.1");
        }

        [TestMethod]
        public void Test_Enumerate_OpcDa20_Servers()
        {
            var enumerator = new OpcServerEnumeratorAuto();
            var serverDescriptions = enumerator.Enumerate(null, OpcServerCategory.OpcDaServer20);
            serverDescriptions.Should().ContainSingle(s => s.ProgId == "Matrikon.OPC.Simulation.1");
            serverDescriptions.Should().ContainSingle(s => s.Uri.Segments[1] == "Matrikon.OPC.Simulation.1");
        }

        [TestMethod]
        public void Test_Enumerate_OpcDa30_Servers()
        {
            var enumerator = new OpcServerEnumeratorAuto();
            var serverDescriptions = enumerator.Enumerate("localhost", OpcServerCategory.OpcDaServer30);
            serverDescriptions.Should().ContainSingle(s => s.ProgId == "Matrikon.OPC.Simulation.1");
            serverDescriptions.Should().ContainSingle(s => s.Uri.Segments[1] == "Matrikon.OPC.Simulation.1");
        }

        [TestMethod]
        public void Test_Enumerate_OpcDa_Servers()
        {
            var enumerator = new OpcServerEnumeratorAuto();
            var serverDescriptions = enumerator.Enumerate("", OpcServerCategory.OpcDaServers);
            serverDescriptions.Should().ContainSingle(s => s.ProgId == "Matrikon.OPC.Simulation.1");
            serverDescriptions.Should().ContainSingle(s => s.Uri.Segments[1] == "Matrikon.OPC.Simulation.1");
        }

        [TestMethod]
        public void Test_Enumerate_Hosts()
        {
            var enumerator = new OpcServerEnumeratorAuto();
            var hosts = enumerator.EnumrateHosts();
            hosts.Should().Contain(enumerator.Localhost);
        }
    }
}