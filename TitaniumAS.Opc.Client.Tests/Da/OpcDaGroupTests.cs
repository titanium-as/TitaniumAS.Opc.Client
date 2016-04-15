using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using TitaniumAS.Opc.Client.Interop.Da;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Tests.Da
{
    [TestClass]
    public class OpcDaGroupTests
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
        public void Test_Culture()
        {
            var group = _server.AddGroup("g1");
            var culture = group.Culture;
            group.Culture = CultureInfo.InvariantCulture;
            // matrikon doesn't support LCID
        }

        [TestMethod]
        public void Test_IsActive()
        {
            var group = _server.AddGroup("g1");
            var value = group.IsActive;
            group.IsActive = !value;
            group.IsActive.Should().Be(!value);
        }

        [TestMethod]
        public void Test_Name()
        {
            var group = _server.AddGroup("g1");
            var value = group.Name;
            group.Name = "BBB";
            group.Name.Should().Be("BBB");
        }

        [TestMethod]
        public void Test_PercentDeadband()
        {
            var group = _server.AddGroup("g1");
            var value = group.PercentDeadband;
            group.PercentDeadband = 50.0f;
            group.PercentDeadband.Should().Be(50.0f);
        }

        [TestMethod]
        public void Test_TimeBias()
        {
            var group = _server.AddGroup("g1");
            var value = group.TimeBias;
            group.TimeBias = TimeSpan.FromHours(-5);
            group.TimeBias.Should().Be(TimeSpan.FromHours(-5));
        }

        [TestMethod]
        public void Test_UpdateRate()
        {
            var group = _server.AddGroup("g1");
            var value = group.UpdateRate;
            group.UpdateRate = TimeSpan.FromMilliseconds(5000);
            group.UpdateRate.Should().Be(TimeSpan.FromMilliseconds(5000));
        }

        [TestMethod]
        public void Test_ClientHandle()
        {
            var group = _server.AddGroup("g1");
            var value = group.ClientHandle;
            group.ClientHandle = 11;
            group.ClientHandle.Should().Be(11);
        }

        [TestMethod]
        public void Test_ServerHandle()
        {
            var group = _server.AddGroup("g1");
            var value = group.ServerHandle;
            value.Should().NotBe(0);
        }

        [TestMethod]
        public void Test_KeepAlive()
        {
            var group = _server.AddGroup("g1");
            var value = group.KeepAlive;
            group.KeepAlive = TimeSpan.FromMilliseconds(5000);
            group.KeepAlive.Should().Be(TimeSpan.FromMilliseconds(5000));
        }

        [TestMethod]
        public void Test_Clone()
        {
            var g1 = _server.AddGroup("g1");
            g1.IsActive = true;
            g1.UpdateRate = TimeSpan.FromMilliseconds(666);
            g1.PercentDeadband = 33;

            var g2 = g1.Clone("g2");

            _server.Groups.Should().Contain(g2);
            g2.Name.Should().Be("g2");
            g2.UpdateRate.Should().Be(g1.UpdateRate);
            g2.PercentDeadband.Should().Be(g1.PercentDeadband);
            g2.IsActive.Should().Be(false);
        }

        [TestMethod]
        public void Test_Clone_With_Items()
        {
            var g1 = _server.AddGroup("g1");
            g1.AddItems(new[] {new OpcDaItemDefinition {ItemId = "Random.Int2"}});

            var g2 = g1.Clone("g2");
            g2.Items.Should().HaveCount(1);
            g2.Items[0].ItemId.Should().Be("Random.Int2");
        }

        [TestMethod]
        public void Test_AddItem()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                }
            };
            var opcItemResults = g1.AddItems(items);
            opcItemResults.Should().HaveCount(1);

            var opcItemResult = opcItemResults[0];
            opcItemResult.Error.Succeeded.Should().BeTrue();
            opcItemResult.Item.AccessRights.Should().Be(OpcDaAccessRights.Read);

            g1.Items.Should().HaveCount(1);
            var item = g1.Items[0];
            item.ItemId.Should().Be("Random.Int1");
            item.IsActive.Should().Be(true);
            item.AccessRights.Should().Be(OpcDaAccessRights.Read);
        }

        [TestMethod]
        public void Test_AddItems()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                }
            };
            var opcItemResults = g1.AddItems(items);
            opcItemResults.Should().HaveCount(2);
            g1.Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void Test_ValidateItems()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                },
                new OpcDaItemDefinition
                {
                    ItemId = "BadItemId",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true,
                    RequestedDataType =  TypeConverter.FromVarEnum(VarEnum.VT_ARRAY)
                }
            };
            var opcItemResults = g1.ValidateItems(items);
            opcItemResults.Should().HaveCount(3);
            opcItemResults[0].Error.Succeeded.Should().BeTrue();
            opcItemResults[1].Error.Should().Be(HRESULT.OPC_E_INVALIDITEMID);
            opcItemResults[2].Error.Should().Be(HRESULT.OPC_E_BADTYPE);
        }

        [TestMethod]
        public void Test_RemoveItems()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                }
            };
            g1.AddItems(items);
            g1.RemoveItems(g1.Items);
            g1.Items.Should().HaveCount(0);

            g1.SyncItems();
            g1.Items.Should().HaveCount(0);
        }

        [TestMethod]
        public void Test_EnumerateItems()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY)
                }
            };
            g1.AddItems(items);
            g1.SyncItems();
            g1.Items.Should().HaveCount(2);
        }

        [TestMethod]
        public void Test_Read()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var values = g1.Read(g1.Items, OpcDaDataSource.Device);
            values.Should().HaveCount(2);
            values.Should().OnlyContain(v => v.Error.Succeeded);
            values.Should().OnlyContain(v => v.Quality.Master == OpcDaQualityMaster.Good);
        }

        [TestMethod]
        public void Test_Write()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var errors = g1.Write(g1.Items, new object[] {1, 2.0});
            errors.Should().HaveCount(2);
            errors.Should().OnlyContain(e => e.Succeeded);
        }

        [TestMethod]
        public void Test_ReadMaxAge()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;
            var values = g1.ReadMaxAge(g1.Items, new[] {TimeSpan.Zero, TimeSpan.MaxValue, TimeSpan.FromSeconds(5)});
            values.Should().HaveCount(3);
            values.Should().OnlyContain(v => v.Error.Succeeded);
        }

        [TestMethod]
        public void Test_WriteVQT()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var opcItemValues = new[]
            {
                new OpcDaItemValue
                {
                    Value = 1,
                    Quality = (short) OPC_QUALITY_STATUS.UNCERTAIN,
                    Timestamp = DateTimeOffset.Now
                },
                new OpcDaItemValue
                {
                    Value = 1.7,
                    Quality = (short) OPC_QUALITY_STATUS.UNCERTAIN,
                    Timestamp = DateTimeOffset.Now
                }
            };
            var errors = g1.WriteVQT(g1.Items, opcItemValues);
            errors.Should().HaveCount(2);
            errors.Should().OnlyContain(e => e.Succeeded);
        }

        [TestMethod]
        public void Test_ReadAsync()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource();
            var task = g1.ReadAsync(g1.Items, cts.Token);
            task.Wait();
            var values = task.Result;
            values.Should().HaveCount(2);
            values.Should().OnlyContain(v => v.Error.Succeeded);
            values.Should().OnlyContain(v => v.Quality.Master == OpcDaQualityMaster.Good);
        }

        [TestMethod]
        public void Test_ReadAsync_Cancellation()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                }
            };
            g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
            var task = g1.ReadAsync(g1.Items, cts.Token);

            Action act = () => task.Wait();
            act.ShouldThrow<AggregateException>();
        }

        [TestMethod]
        public void Test_RefreshAsync()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource();
            var task = g1.RefreshAsync(OpcDaDataSource.Device, cts.Token);
            task.Wait();
            var values = task.Result;
            values.Should().HaveCount(2);
            values.Should().OnlyContain(v => v.Error.Succeeded);
            values.Should().OnlyContain(v => v.Quality.Master == OpcDaQualityMaster.Good);
        }

        [TestMethod]
        public void Test_RefreshAsync_Cancellation()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                }
            };
            g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
            cts.Cancel();
            var task = g1.RefreshAsync(OpcDaDataSource.Device, cts.Token);

            Action act = () => task.Wait();
            act.ShouldThrow<AggregateException>();
        }

        [TestMethod]
        public void Test_WriteAsync()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource();
            var task = g1.WriteAsync(g1.Items, new object[] {1, 2.0}, cts.Token);
            task.Wait();
            var errors = task.Result;
            errors.Should().HaveCount(2);
            errors.Should().OnlyContain(e => e.Succeeded);
        }

        [TestMethod]
        public void Test_WriteAsync_Cancellation()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true
                }
            };
            g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
            var task = g1.WriteAsync(g1.Items, new object[] {1, 2.0}, cts.Token);
            Action act = () => task.Wait();
            act.ShouldThrow<AggregateException>();
        }

        [TestMethod]
        public void Test_ReadMaxAgeAsync()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource();
            var task = g1.ReadMaxAgeAsync(g1.Items, new[] {TimeSpan.Zero, TimeSpan.Zero}, cts.Token);
            task.Wait();
            var values = task.Result;
            values.Should().HaveCount(2);
            values.Should().OnlyContain(v => v.Error.Succeeded);
        }

        [TestMethod, Ignore]
        public void Test_ReadMaxAgeAsync_Cancellation()
        {
            var g1 = _server.AddGroup("g1");
            var items = Enumerable.Repeat(
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                }, 10000
                ).ToList();
            g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
            cts.Cancel();
            var task = g1.ReadMaxAgeAsync(g1.Items, null, cts.Token);
            Action act = task.Wait;
            act.ShouldThrow<AggregateException>();
        }

        [TestMethod]
        public void Test_RefreshMaxAgeAsync()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource();
            var task = g1.RefreshMaxAgeAsync(TimeSpan.Zero, cts.Token);
            task.Wait();
            var values = task.Result;
            values.Should().HaveCount(2);
            values.Should().OnlyContain(v => v.Error.Succeeded);
            values.Should().OnlyContain(v => v.Quality.Master == OpcDaQualityMaster.Good);
        }

        [TestMethod , Ignore]
        public void Test_RefreshMaxAgeAsync_Cancellation()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = true
                }
            };
            g1.AddItems(items);
            g1.IsActive = true;

            var cts = new CancellationTokenSource();
            cts.Cancel();
            var task = g1.RefreshMaxAgeAsync(TimeSpan.Zero, cts.Token);

            Action act = task.Wait;
            act.ShouldThrow<AggregateException>();
        }

        [TestMethod]
        public void Test_WriteVQTAsync()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int2",
                    IsActive = true,
                    RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                }
            };
            var itemResult = g1.AddItems(items);
            g1.IsActive = true;

            var opcItemValues = new[]
            {
                new OpcDaItemValue
                {
                    Value = 1,
                    Quality = (short) OPC_QUALITY_STATUS.UNCERTAIN,
                    Timestamp = DateTimeOffset.Now
                },
                new OpcDaItemValue
                {
                    Value = 1.7,
                    Quality = (short) OPC_QUALITY_STATUS.UNCERTAIN,
                    Timestamp = DateTimeOffset.Now
                }
            };
            var cts = new CancellationTokenSource();
            var task = g1.WriteVQTAsync(g1.Items, opcItemValues, cts.Token);
            task.Wait();
            var errors = task.Result;
            errors.Should().HaveCount(2);
            errors.Should().OnlyContain(e => e.Succeeded);
        }

        [TestMethod]
        public void Test_WriteVQTAsync_Cancellation()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Write only.Int1",
                    IsActive = true
                }
            };
            g1.AddItems(items);
            g1.IsActive = true;

            var opcItemValues = new[]
            {
                new OpcDaItemValue
                {
                    Value = 1,
                    Quality = (short) OPC_QUALITY_STATUS.UNCERTAIN,
                    Timestamp = DateTimeOffset.Now
                }
            };
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
            cts.Cancel();
            var task = g1.WriteVQTAsync(g1.Items, opcItemValues, cts.Token);
            Action act = task.Wait;
            act.ShouldThrow<AggregateException>();
        }

        [TestMethod]
        public void Test_IsSubscribed()
        {
            var g1 = _server.AddGroup("g1");
            var items = new[]
            {
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int1",
                    IsActive = false
                },
                new OpcDaItemDefinition
                {
                    ItemId = "Random.Int2",
                    IsActive = false
                }
            };
            g1.UpdateRate = TimeSpan.FromMilliseconds(500);
            g1.IsSubscribed = true;
            
            var values = new List<OpcDaItemValue>();
            AutoResetEvent callbackCalled = new AutoResetEvent(false);
            g1.ValuesChanged += delegate(object sender, OpcDaItemValuesChangedEventArgs args)
            {
                values.InsertRange(values.Count, args.Values);
                callbackCalled.Set();
            };

            g1.AddItems(items);
            var v1 = g1.Items[0];
            var v2 = g1.Items[1];

            // 1st active
            g1.SetActiveItems(new []{v1});
            g1.IsActive = true;

            callbackCalled.WaitOne(TimeSpan.FromSeconds(5));
            g1.IsActive = false;
            values.Should().OnlyContain(v => v.Item == v1).And.NotBeEmpty();

            // 2nd active
            g1.SetActiveItems(new []{v1}, false);
            g1.SetActiveItems(new []{v2}, true);
            values.Clear();
            g1.IsActive = true;

            callbackCalled.WaitOne(TimeSpan.FromSeconds(5));
            g1.IsActive = false;
            values.Should().OnlyContain(v => v.Item == v2).And.NotBeEmpty();

            // all active    
            g1.SetActiveItems(g1.Items);
            values.Clear();
            g1.IsActive = true;

            callbackCalled.WaitOne(TimeSpan.FromSeconds(5));
            g1.IsActive = false;
            values.Should().Contain(v => v.Item == v1).And.Contain(v => v.Item == v2).And.NotBeEmpty();
        }

        [TestMethod]
        public void Test_PendingRequests()
        {
            using (var server = new OpcDaServer(UrlBuilder.Build("Matrikon.OPC.Simulation.1")))
            {
                server.Connect();
                var group = server.AddGroup("g");
                var itemDefs = Enumerable.Repeat(
                    new[]
                        {
                            new OpcDaItemDefinition
                            {
                                ItemId = "Random.Int1"
                            },
                            new OpcDaItemDefinition
                            {
                                ItemId = "Random.Int2",
                                RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                            }
                        }, 1000).SelectMany(t => t).ToArray();
                group.AddItems(itemDefs);
                group.SetActiveItems(group.Items);
                group.IsActive = true;
                group.UpdateRate = TimeSpan.FromMilliseconds(10);
                group.IsSubscribed = true;
                CancellationTokenSource cts = new CancellationTokenSource();
                
                List<Task> tasks = new List<Task>();
                for (var i = 0; i < 100; i++)
                {
                    tasks.Add(group.ReadAsync(group.Items, cts.Token));
                    tasks.Add(group.ReadMaxAgeAsync(group.Items, TimeSpan.Zero, cts.Token));
                    tasks.Add(group.RefreshAsync(OpcDaDataSource.Device, cts.Token));
                    tasks.Add(group.RefreshMaxAgeAsync(TimeSpan.Zero, cts.Token));
                    tasks.Add(group.WriteAsync(group.Items, Enumerable.Repeat((object)1, group.Items.Count).ToArray(), cts.Token));
                    tasks.Add(group.WriteVQTAsync(group.Items, Enumerable.Repeat(new OpcDaVQT(), group.Items.Count).ToArray(), cts.Token));
                }

                Task.WaitAll(tasks.ToArray());
            }
            GC.Collect();
        }

        [TestMethod, Ignore]
        public void Test_MemoryLeaks()
        {
            using (var server = new OpcDaServer(UrlBuilder.Build("Matrikon.OPC.Simulation.1")))
            {
                for (var gi = 0; gi < 500; gi++)
                {
                    var group = server.AddGroup("g" + gi);
                    var itemDefs = Enumerable.Repeat(
                        new[]
                        {
                            new OpcDaItemDefinition
                            {
                                ItemId = "Random.Int1"
                            },
                            new OpcDaItemDefinition
                            {
                                ItemId = "Random.Int2",
                                RequestedDataType = TypeConverter.FromVarEnum(VarEnum.VT_R4)
                            }
                        }, 1000).SelectMany(t => t).ToArray();

                    var cts = new CancellationTokenSource();

                    group.ValidateItems(itemDefs);
                    group.AddItems(itemDefs);
                    group.SetActiveItems(group.Items);
                    group.SetDataTypes(group.Items, TypeConverter.FromVarEnum(VarEnum.VT_I4));
                    group.SyncItems();

                    group.IsActive = true;
                    group.SyncState();

                    // sync read
                    group.Read(group.Items);
                    group.ReadMaxAge(group.Items, TimeSpan.Zero);

                    // async read
                    Task.WaitAll(group.ReadAsync(group.Items, cts.Token), group.ReadMaxAgeAsync(group.Items,
                        TimeSpan.Zero, cts.Token),
                        group.RefreshAsync(OpcDaDataSource.Cache, cts.Token)
                        );

                    group.RemoveItems(group.Items.Take(group.Items.Count/2).ToArray());

                    // sync read
                    group.Read(group.Items);
                    group.ReadMaxAge(group.Items, TimeSpan.Zero);

                    // async read
                    Task.WaitAll(group.ReadAsync(group.Items, cts.Token), group.ReadMaxAgeAsync(group.Items,
                        TimeSpan.Zero, cts.Token),
                        group.RefreshAsync(OpcDaDataSource.Cache, cts.Token)
                        );
                }
            }
            GC.Collect();
        }
    }
}