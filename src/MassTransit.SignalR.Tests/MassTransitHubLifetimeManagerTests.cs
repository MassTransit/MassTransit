namespace MassTransit.SignalR.Tests
{
    using MassTransit.SignalR.Contracts;
    using MassTransit.SignalR.Tests.OfficialFramework;
    using MassTransit.Testing;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using NUnit.Framework;
    using System.Linq;
    using System.Threading.Tasks;

    public class MassTransitHubLifetimeManagerTests : MassTransitHubLifetimeTestFixture<MyHub>
    {
        protected override BusTestHarness Harness { get; set; }

        public class TestObject
        {
            public string TestProperty { get; set; }
        }

        [Test]
        public async Task CamelCasedJsonIsPreservedAcrossMassTransitBoundary()
        {
            Harness = new InMemoryTestHarness();

            var messagePackOptions = new MessagePackHubProtocolOptions();

            var jsonOptions = new JsonHubProtocolOptions();
            jsonOptions.PayloadSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var backplane1Harness = RegisterBusEndpoint("receiveEndpoint1");
            var backplane2Harness = RegisterBusEndpoint("receiveEndpoint2");

            await Harness.Start();

            try
            {
                backplane1Harness.SetHubLifetimeManager(CreateLifetimeManager(messagePackOptions, jsonOptions));
                backplane2Harness.SetHubLifetimeManager(CreateLifetimeManager());
                using (var client1 = new TestClient())
                using (var client2 = new TestClient())
                {
                    var manager1 = backplane1Harness.HubLifetimeManager;
                    var manager2 = backplane2Harness.HubLifetimeManager;

                    var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                    var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                    await manager1.OnConnectedAsync(connection1).OrTimeout();
                    await manager2.OnConnectedAsync(connection2).OrTimeout();

                    await manager1.SendAllAsync("Hello", new object[] { new TestObject { TestProperty = "Foo" } });

                    Assert.IsTrue(backplane2Harness.All.Consumed.Select<All<MyHub>>().Any());

                    var message = await client2.ReadAsync().OrTimeout() as InvocationMessage;
                    Assert.NotNull(message);
                    Assert.AreEqual("Hello", message.Target);
                    CollectionAssert.AllItemsAreInstancesOfType(message.Arguments, typeof(JObject));
                    var jObject = message.Arguments[0] as JObject;
                    Assert.NotNull(jObject);
                    var firstProperty = jObject.Properties().First();
                    Assert.AreEqual("testProperty", firstProperty.Name);
                    Assert.AreEqual("Foo", firstProperty.Value.Value<string>());
                }
            }
            finally
            {
                await Harness.Stop();
            }
        }
    }
}
