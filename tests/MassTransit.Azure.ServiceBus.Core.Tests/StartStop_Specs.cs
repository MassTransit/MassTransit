namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class StartStop_Specs :
        BusTestFixture
    {
        [Test]
        public async Task Should_start_stop_and_start()
        {
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();
            var serviceBusNamespace = Configuration.ServiceNamespace;

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(
                serviceBusNamespace,
                "MassTransit.Azure.ServiceBus.Core.Tests"
            );
            var bus = MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                x.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });
                ConfigureBusDiagnostics(x);

                x.ReceiveEndpoint("input_queue", e =>
                {
                    e.Handler<PingMessage>(async context =>
                    {
                        await context.RespondAsync(new PongMessage());
                    });
                });
            });

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.CreateRequestClient<PingMessage>().GetResponse<PongMessage>(new PingMessage());
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }

            await bus.StartAsync(TestCancellationToken);
            try
            {
                await bus.CreateRequestClient<PingMessage>().GetResponse<PongMessage>(new PingMessage());
            }
            finally
            {
                await bus.StopAsync(TestCancellationToken);
            }
        }

        public StartStop_Specs()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
