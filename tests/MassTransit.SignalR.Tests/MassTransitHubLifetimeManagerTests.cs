namespace MassTransit.SignalR.Tests
{
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Protocol;
    using NUnit.Framework;
    using OfficialFramework;
    using Testing;


    public class MassTransitHubLifetimeManagerTests : MassTransitHubLifetimeTestFixture<MyHub>
    {
        protected override BusTestHarness Harness { get; set; }

        [Test]
        public async Task CamelCasedJsonIsPreservedAcrossMassTransitBoundary()
        {
            Harness = new InMemoryTestHarness();

            var messagePackOptions = new MessagePackHubProtocolOptions();

            var jsonOptions = new JsonHubProtocolOptions();
            jsonOptions.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            SignalRBackplaneConsumersTestHarness<MyHub> backplane1Harness = RegisterBusEndpoint("receiveEndpoint1");
            SignalRBackplaneConsumersTestHarness<MyHub> backplane2Harness = RegisterBusEndpoint("receiveEndpoint2");

            await Harness.Start();

            try
            {
                backplane1Harness.SetHubLifetimeManager(CreateLifetimeManager(messagePackOptions, jsonOptions));
                backplane2Harness.SetHubLifetimeManager(CreateLifetimeManager());
                using (var client1 = new TestClient())
                using (var client2 = new TestClient())
                {
                    MassTransitHubLifetimeManager<MyHub> manager1 = backplane1Harness.HubLifetimeManager;
                    MassTransitHubLifetimeManager<MyHub> manager2 = backplane2Harness.HubLifetimeManager;

                    var connection1 = HubConnectionContextUtils.Create(client1.Connection);
                    var connection2 = HubConnectionContextUtils.Create(client2.Connection);

                    await manager1.OnConnectedAsync(connection1).OrTimeout(Harness.TestTimeout);
                    await manager2.OnConnectedAsync(connection2).OrTimeout(Harness.TestTimeout);

                    await manager1.SendAllAsync("Hello", new object[] { new TestObject { TestProperty = "Foo" } }).OrTimeout(Harness.TestTimeout);

                    Assert.That(backplane2Harness.All.Consumed.Select<All<MyHub>>().Any(), Is.True);

                    var message = await client2.ReadAsync().OrTimeout() as InvocationMessage;
                    Assert.That(message, Is.Not.Null);
                    Assert.Multiple(() =>
                    {
                        Assert.That(message.Target, Is.EqualTo("Hello"));
                        Assert.That(message.Arguments, Is.All.InstanceOf(typeof(JsonElement)));
                    });
                    var jsonElement = message.Arguments[0] as JsonElement?;
                    Assert.That(jsonElement, Is.Not.Null);
                    var testProperty = jsonElement.Value.GetProperty("testProperty");
                    Assert.That(testProperty.GetString(), Is.EqualTo("Foo"));
                }
            }
            finally
            {
                await Harness.Stop();
            }
        }


        public class TestObject
        {
            public string TestProperty { get; set; }
        }
    }
}
