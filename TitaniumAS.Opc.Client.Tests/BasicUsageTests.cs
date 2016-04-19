using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Da.Browsing;

namespace TitaniumAS.Opc.Client.Tests
{
    [TestClass]
    public class BasicUsageTests
    {
        [TestMethod]
        public void BrowsingAnOpcDaServerLocally()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Browse elements.
                var browser = new OpcDaBrowserAuto(server);
                BrowseChildren(browser);

                // The output should be like the following:
                // #MonitorACLFile (ItemId: #MonitorACLFile, IsHint: False, IsItem: True, HasChildren: False)
                // @Clients (ItemId: @Clients, IsHint: False, IsItem: True, HasChildren: False)
                // Configured Aliases (ItemId: Configured Aliases, IsHint: False, IsItem: False, HasChildren: True)
                // Simulation Items (ItemId: Simulation Items, IsHint: False, IsItem: False, HasChildren: True)
                //   Bucket Brigade (ItemId: Bucket Brigade, IsHint: False, IsItem: False, HasChildren: True)
                //     ArrayOfReal8 (ItemId: Bucket Brigade.ArrayOfReal8, IsHint: False, IsItem: True, HasChildren: False)
                //     ArrayOfString (ItemId: Bucket Brigade.ArrayOfString, IsHint: False, IsItem: True, HasChildren: False)
                //     Boolean (ItemId: Bucket Brigade.Boolean, IsHint: False, IsItem: True, HasChildren: False)
                //     Int1 (ItemId: Bucket Brigade.Int1, IsHint: False, IsItem: True, HasChildren: False)
                //     Int2 (ItemId: Bucket Brigade.Int2, IsHint: False, IsItem: True, HasChildren: False)
                //     Int4 (ItemId: Bucket Brigade.Int4, IsHint: False, IsItem: True, HasChildren: False)
                // ...
            }
        }

        private static void BrowseChildren(IOpcDaBrowser browser, string itemId = null, int indent = 0)
        {
            // Browse elements.
            // When itemId is null, root elements will be browsed.
            OpcDaBrowseElement[] elements = browser.GetElements(itemId);

            // Output elements.
            foreach (OpcDaBrowseElement element in elements)
            {
                // Output the element.
                Console.Write(new String(' ', indent));
                Console.WriteLine(element);

                // Skip elements without children.
                if (!element.HasChildren)
                    continue;

                // Output children of the element.
                BrowseChildren(browser, element.ItemId, indent + 2);
            }
        }

        [TestMethod]
        public void CreatingAGroupWithItemsInAnOpcDaServer()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Create a group with items.
                CreateGroupWithItems(server);

                server.Groups.Should().HaveCount(1);
                server.Groups[0].Name.Should().Be("MyGroup");
                server.Groups[0].Items.Should().HaveCount(2);
                server.Groups[0].Items[0].ItemId.Should().Be("Bucket Brigade.Int4");
                server.Groups[0].Items[1].ItemId.Should().Be("Random.Int2");
            }
        }

        private static OpcDaGroup CreateGroupWithItems(OpcDaServer server)
        {
            // Create a group with items.
            OpcDaGroup group = server.AddGroup("MyGroup");
            group.IsActive = true;

            var definition1 = new OpcDaItemDefinition
            {
                ItemId = "Bucket Brigade.Int4",
                IsActive = true
            };
            var definition2 = new OpcDaItemDefinition
            {
                ItemId = "Random.Int2",
                IsActive = true
            };
            OpcDaItemDefinition[] definitions = {definition1, definition2};
            OpcDaItemResult[] results = group.AddItems(definitions);

            // Handle adding results.
            foreach (OpcDaItemResult result in results)
            {
                if (result.Error.Failed)
                    Console.WriteLine("Error adding items: {0}", result.Error);
            }

            return group;
        }

        [TestMethod]
        public void ReadValiesSynchronously()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Create a group with items.
                OpcDaGroup group = CreateGroupWithItems(server);

                // Read values of items from device synchronously.
                OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);

                // Output values
                foreach (OpcDaItemValue value in values)
                {
                    Console.WriteLine("ItemId: {0}; Value: {1}; Quality: {2}; Timestamp: {3}",
                        value.Item.ItemId, value.Value, value.Quality, value.Timestamp);
                }

                // The output should be like the following:
                //   ItemId: Bucket Brigade.Int4; Value: 0; Quality: Good+Good+NotLimited; Timestamp: 04/18/2016 13:40:57 +03:00
                //   ItemId: Random.Int2; Value: 26500; Quality: Good+Good+NotLimited; Timestamp: 04/18/2016 13:40:57 +03:00

                values.Should().OnlyContain(v => v.Error.Succeeded);
                values.Should().OnlyContain(v => v.Quality.Master == OpcDaQualityMaster.Good);
            }
        }

        [TestMethod]
        public async Task ReadValiesAsynchronously()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Create a group with items.
                OpcDaGroup group = CreateGroupWithItems(server);

                // Read values of items from device asynchronously.
                OpcDaItemValue[] values = await group.ReadAsync(group.Items);

                // Output values
                foreach (OpcDaItemValue value in values)
                {
                    Console.WriteLine("ItemId: {0}; Value: {1}; Quality: {2}; Timestamp: {3}",
                        value.Item.ItemId, value.Value, value.Quality, value.Timestamp);
                }

                // The output should be like the following:
                //   ItemId: Bucket Brigade.Int4; Value: 0; Quality: Good+Good+NotLimited; Timestamp: 04/18/2016 13:40:57 +03:00
                //   ItemId: Random.Int2; Value: 26500; Quality: Good+Good+NotLimited; Timestamp: 04/18/2016 13:40:57 +03:00

                values.Should().OnlyContain(v => v.Error.Succeeded);
                values.Should().OnlyContain(v => v.Quality.Master == OpcDaQualityMaster.Good);
            }
        }

        [TestMethod]
        public void WriteValiesToAnItemOfAnOpcServerSynchronously()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Create a group with items.
                OpcDaGroup group = CreateGroupWithItems(server);

                // Write value to the item.
                OpcDaItem item = group.Items.FirstOrDefault(i => i.ItemId == "Bucket Brigade.Int4");
                OpcDaItem[] items = { item };
                object[] values = { 123 };

                HRESULT[] results = group.Write(items, values);

                // Handle write result.
                if (results[0].Failed)
                {
                    Console.WriteLine("Error writing value");
                }

                // Read and output value.
                OpcDaItemValue value = group.Read(items, OpcDaDataSource.Device)[0];
                Console.WriteLine("ItemId: {0}; Value: {1}; Quality: {2}; Timestamp: {3}",
                    value.Item.ItemId, value.Value, value.Quality, value.Timestamp);

                // The output should be like the following:
                //   ItemId: Bucket Brigade.Int4; Value: 123; Quality: Good+Good+NotLimited; Timestamp: 04/18/2016 14:04:11 +03:00

                value.Value.Should().Be(123);
                value.Quality.Master.Should().Be(OpcDaQualityMaster.Good);
            }
        }

        [TestMethod]
        public async Task WriteValiesToAnItemOfAnOpcServerAsynchronously()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Create a group with items.
                OpcDaGroup group = CreateGroupWithItems(server);

                // Write value to the item.
                OpcDaItem item = group.Items.FirstOrDefault(i => i.ItemId == "Bucket Brigade.Int4");
                OpcDaItem[] items = { item };
                object[] values = { 123 };

                HRESULT[] results = await group.WriteAsync(items, values);

                // Handle write result.
                if (results[0].Failed)
                {
                    Console.WriteLine("Error writing value");
                }

                // Read and output value.
                OpcDaItemValue value = group.Read(items, OpcDaDataSource.Device)[0];
                Console.WriteLine("ItemId: {0}; Value: {1}; Quality: {2}; Timestamp: {3}",
                    value.Item.ItemId, value.Value, value.Quality, value.Timestamp);

                // The output should be like the following:
                //   ItemId: Bucket Brigade.Int4; Value: 123; Quality: Good+Good+NotLimited; Timestamp: 04/18/2016 14:04:11 +03:00

                value.Value.Should().Be(123);
                value.Quality.Master.Should().Be(OpcDaQualityMaster.Good);
            }
        }

        [TestMethod]
        public async Task GetAValueOfAnItemBySubscription()
        {
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");
            using (var server = new OpcDaServer(url))
            {
                // Connect to the server first.
                server.Connect();

                // Create a group with items.
                OpcDaGroup group = CreateGroupWithItems(server);

                // Configure subscription.
                group.ValuesChanged += OnGroupValuesChanged;
                group.UpdateRate = TimeSpan.FromMilliseconds(100); // ValuesChanged won't be triggered if zero

                // Wait some time.
                await Task.Delay(1000);

                // The output should be like the following:
                //   ItemId: Bucket Brigade.Int4; Value: 0; Quality: Good+Good+NotLimited; Timestamp: 04/19/2016 12:41:11 +03:00
                //   ItemId: Random.Int2; Value: 0; Quality: Bad+BadOutOfService+NotLimited; Timestamp: 04/19/2016 12:41:11 +03:00
                //   ItemId: Random.Int2; Value: 41; Quality: Good+Good+NotLimited; Timestamp: 04/19/2016 12:41:13 +03:00
                //   ItemId: Random.Int2; Value: 18467; Quality: Good+Good+NotLimited; Timestamp: 04/19/2016 12:41:13 +03:00
                // ...
            }
        }

        private void OnGroupValuesChanged(object sender, OpcDaItemValuesChangedEventArgs args)
        {
            // Output values.
            foreach (OpcDaItemValue value in args.Values)
            {
                Console.WriteLine("ItemId: {0}; Value: {1}; Quality: {2}; Timestamp: {3}",
                    value.Item.ItemId, value.Value, value.Quality, value.Timestamp);
            }
        }
    }
}
