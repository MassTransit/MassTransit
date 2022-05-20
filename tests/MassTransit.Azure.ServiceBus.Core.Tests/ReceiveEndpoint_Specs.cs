namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = Bus.ConnectReceiveEndpoint("second_queue", x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });

            await handle.Ready;

            try
            {
                await Bus.Publish(new PingMessage());

                ConsumeContext<PingMessage> pinged = await pingHandled;

                Assert.That(pinged.ReceiveContext.InputAddress,
                    Is.EqualTo(new Uri(string.Join("/", HostAddress.GetLeftPart(UriPartial.Path), "second_queue"))));
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            var handle = Bus.ConnectReceiveEndpoint("second_queue", x =>
            {
                Handled<PingMessage>(x);
            });

            await handle.Ready;

            try
            {
                Assert.That(async () =>
                {
                    var unused = Bus.ConnectReceiveEndpoint("second_queue", x =>
                    {
                    });

                    await unused.Ready;
                }, Throws.TypeOf<ConfigurationException>());
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_work_when_reconnected()
        {
            async Task ConnectAndConsume()
            {
                var correlationId = NewId.NextGuid();

                Task<ConsumeContext<PingMessage>> pingHandled = null;

                var handle = Bus.ConnectReceiveEndpoint("second_queue", x =>
                {
                    pingHandled = Handled<PingMessage>(x, context => context.Message.CorrelationId == correlationId);

                    ((IServiceBusReceiveEndpointConfigurator)x).RemoveSubscriptions = true;
                });

                await handle.Ready;

                try
                {
                    await Bus.Publish(new PingMessage(correlationId));

                    ConsumeContext<PingMessage> pinged = await pingHandled;

                    Assert.That(pinged.ReceiveContext.InputAddress,
                        Is.EqualTo(new Uri(string.Join("/", HostAddress.GetLeftPart(UriPartial.Path), "second_queue"))));
                }
                finally
                {
                    await handle.StopAsync();
                }
            }

            await ConnectAndConsume();
            await ConnectAndConsume();
        }

        public Creating_a_receive_endpoint_from_an_existing_bus()
        {
            TestTimeout = TimeSpan.FromMinutes(1);
        }
    }


    public class Connecting_a_temporary_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_work_as_expected()
        {
            var connector = _provider.GetRequiredService<IReceiveEndpointConnector>();

            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = connector.ConnectReceiveEndpoint(new TemporaryEndpointDefinition(), KebabCaseEndpointNameFormatter.Instance, (context, cfg) =>
            {
                pingHandled = Handled<PingMessage>(cfg);
            });

            await handle.Ready;

            try
            {
                await Bus.Publish(new PingMessage());

                ConsumeContext<PingMessage> pinged = await pingHandled;
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        readonly IServiceProvider _provider;

        public Connecting_a_temporary_endpoint()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(ConfigureRegistration)
                .BuildServiceProvider(true);
        }

        void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }
    }
}
