using System;
using System.ServiceProcess;
using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Wrappers;
using TitaniumAS.Opc.Client.Interop.Da;

namespace TitaniumAS.Opc.Client.Tests.Da
{
    [TestClass]
    public class OpcServerEnumeratorTests
    {
        private OpcDaServer _server;

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new OpcDaServer(UrlBuilder.Build("Matrikon.OPC.Simulation.1"));
            _server.Connect();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _server.Dispose();
        }

        [TestMethod]
        public void Test_Connect_Disconnect()
        {
            var serv = new OpcDaServer(UrlBuilder.Build("Matrikon.OPC.Simulation.1"));
            serv.Connect();
            serv.IsConnected.Should().BeTrue();
            serv.Disconnect();
            serv.IsConnected.Should().BeFalse();
            serv.Connect();
            serv.IsConnected.Should().BeTrue();
            serv.Disconnect();
            serv.IsConnected.Should().BeFalse();
        }

        [TestMethod]
        public void Test_GetStatus()
        {
            OpcDaServerStatus status = _server.GetStatus();
            status.ServerState.Should().Be(OpcDaServerState.Running);
            status.VendorInfo.Should().Contain("Matrikon");
        }

        [TestMethod]
        public void Test_AddressSpaceBrowser()
        {
            _server.As<OpcBrowseServerAddressSpace>().Should().NotBeNull();
        }

        [TestMethod]
        public void Test_Browser()
        {
            _server.As<OpcBrowse>().Should().NotBeNull();
        }

        [TestMethod]
        public void Test_QueryAvailableLocaleCultures()
        {
            var cultures = _server.QueryAvailableCultures();
        }

        [TestMethod]
        public void Test_Culture()
        {
            var culture = _server.Culture;
        }

        [TestMethod]
        public void Test_AddGroup()
        {
            var group = _server.AddGroup("a1");
            group.Should().NotBeNull();
            _server.Groups.Should().Contain(group).And.HaveCount(1);
            group.Name.Should().Be("a1");
            group.IsActive.Should().Be(false);
            group.Culture.Should().Be(_server.Culture);
            group.PercentDeadband.Should().Be(0.0f);
            group.UpdateRate.Should().Be(TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void Test_SyncGroups()
        {
            var g1 = _server.AddGroup("a1");
            var g2 = _server.AddGroup("a2");
            _server.SyncGroups();
            _server.Groups.Should().Contain(g => g.Name == g1.Name);
            _server.Groups.Should().Contain(g => g.Name == g2.Name);
            foreach (var opcDaGroup in _server.Groups)
            {
                opcDaGroup.SyncItems();
                opcDaGroup.SyncState();
            }
        }

        [TestMethod]
        public void Test_RemoveGroup()
        {
            var group = _server.AddGroup("a1");
            group.Should().NotBeNull();
            _server.RemoveGroup(group);
            _server.Groups.Should().BeEmpty();
        }

        [TestMethod]
        public void Test_Read()
        {
            var values = _server.Read(new[] { "Random.Int1", "Random.Int2" }, new[] { TimeSpan.Zero, TimeSpan.Zero });
            values.Should().OnlyContain(v => v.Error.Succeeded);
        }

        [TestMethod]
        public void Test_WriteVQT()
        {
            string[] itemIds = new[] { "Write Only.Int1", "Write Only.Int2" };
            OpcDaItemValue[] values = new OpcDaItemValue[]
            {
                new OpcDaItemValue()
                {
                    Value = 0,
                    Quality = (short) OPC_QUALITY_STATUS.BAD
                }, 
                new OpcDaItemValue()
                {
                    Value = 0,
                    Timestamp = DateTimeOffset.Now-TimeSpan.FromHours(1)
                }, 
            };
            HRESULT[] errors = _server.WriteVQT(itemIds, values);
            errors.Should().OnlyContain(hres => hres.Succeeded);
        }

        [TestMethod, Ignore]
        public void Test_Shutdown()
        {
            var g1 = _server.AddGroup("g1");
            g1.IsSubscribed = true;
            g1.IsActive = true;

            AutoResetEvent evnt = new AutoResetEvent(false);
            _server.Shutdown += (sender, args) => evnt.Set();
            
            ServiceController sc = new ServiceController("MatrikonOPC Server for Simulation and Testing");
            sc.Stop();
            evnt.WaitOne();
            
            _server.IsConnected.Should().BeFalse();
            _server.Groups.Should().BeEmpty();
        }
    }
}