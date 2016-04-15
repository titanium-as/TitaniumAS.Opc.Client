using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;

namespace TitaniumAS.Opc.Client.Tests.Da
{
    [TestClass]
    public class LoggerTests
    {
        private OpcDaServer _server;

        public LoggerTests()
        {
            var collection = new NameValueCollection();
            collection["level"] = "TRACE";
            LogManager.Adapter = new CapturingLoggerFactoryAdapter();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _server = new OpcDaServer(UrlBuilder.Build("Matrikon.OPC.Simulation.1"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _server.Disconnect();
        }

        [Ignore]
        [TestMethod]
        public void Should_Trace_Log_On_Sucessfully_Call_OPC_Wrapper_Methods()
        {
            //arrange
            var adapter = (CapturingLoggerFactoryAdapter) LogManager.Adapter;

            //act
            _server.Connect();

            //assert
            List<CapturingLoggerEvent> events = adapter.LoggerEvents.ToList();
            events.Should().NotBeEmpty();
            events.Should().NotContain(ev => ev.Exception != null);
            events.Should().Contain(ev => ev.RenderedMessage.Contains("Calling com method:"));
            events.Should().Contain(ev => ev.RenderedMessage.Contains("Success call com method:"));
        }

        [Ignore]
        [TestMethod]
        public void Should_Correct_Stringify_Arguments_On_Call_OPC_Wrapper_Methods()
        {
            //arrange
            var adapter = (CapturingLoggerFactoryAdapter) LogManager.Adapter;
            _server.Connect();

            //act
            _server.Read(new List<string> {"test id"},
                new List<TimeSpan> {TimeSpan.Zero});

            //assert
            List<CapturingLoggerEvent> events = adapter.LoggerEvents.ToList();
            events.Should().NotBeEmpty();
            events.Should().NotContain(ev => ev.Exception != null);
            events.Should().Contain(ev => ev.RenderedMessage.Contains("test id"));
        }
    }
}