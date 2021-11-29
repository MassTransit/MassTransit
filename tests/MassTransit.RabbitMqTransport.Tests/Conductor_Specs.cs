namespace MassTransit.RabbitMqTransport.Tests
{
    namespace ConductorTests
    {
        using System.Threading.Tasks;
        using Contracts;
        using NUnit.Framework;


        namespace Contracts
        {
            using System;


            public interface DeployHappiness
            {
                string Target { get; }
            }


            public interface DeployPayload
            {
                string Target { get; }
            }


            public interface PayloadDeployed
            {
                DateTime Timestamp { get; }
                string Target { get; }
            }
        }


        public class DeployPayloadConsumer :
            IConsumer<DeployPayload>
        {
            public Task Consume(ConsumeContext<DeployPayload> context)
            {
                return context.RespondAsync<PayloadDeployed>(new
                {
                    InVar.Timestamp,
                    context.Message.Target
                });
            }
        }


        [TestFixture]
        public class Using_conductor_for_service_discovery :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_connect_using_the_service_client()
            {
                IRequestClient<DeployPayload> requestClient = Bus.CreateRequestClient<DeployPayload>();

                Response<PayloadDeployed> response = await requestClient.GetResponse<PayloadDeployed>(new { Target = "Bogey" });

                Assert.That(response.Message.Target, Is.EqualTo("Bogey"));
            }

            protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
            {
                configurator.ServiceInstance(instance =>
                {
                    var serviceEndpointName = instance.EndpointNameFormatter.Consumer<DeployPayloadConsumer>();

                    instance.ReceiveEndpoint(serviceEndpointName, x =>
                    {
                        x.Consumer<DeployPayloadConsumer>();
                    });
                });
            }
        }


        [TestFixture]
        public class Linking_a_service_that_later_gets_a_second_instance :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_work()
            {
                var instanceA = await CreateInstance();
                try
                {
                    IRequestClient<DeployPayload> requestClient = Bus.CreateRequestClient<DeployPayload>();

                    Response<PayloadDeployed> response = await requestClient.GetResponse<PayloadDeployed>(new { Target = "A" });

                    Assert.That(response.Message.Target, Is.EqualTo("A"));

                    var instanceB = await CreateInstance();
                    try
                    {
                        await instanceA.StopAsync();

                        response = await requestClient.GetResponse<PayloadDeployed>(new { Target = "B" });

                        Assert.That(response.Message.Target, Is.EqualTo("B"));
                    }
                    finally
                    {
                        await instanceB.StopAsync();
                    }
                }
                finally
                {
                    await instanceA.StopAsync();
                }
            }

            async Task<IBusControl> CreateInstance()
            {
                var bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(HostAddress);

                    var options = new ServiceInstanceOptions()
                        .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

                    cfg.ServiceInstance(options, instance =>
                    {
                        var serviceEndpointName = instance.EndpointNameFormatter.Consumer<DeployPayloadConsumer>();

                        instance.ReceiveEndpoint(serviceEndpointName, x =>
                        {
                            x.Consumer<DeployPayloadConsumer>();
                        });
                    });
                });

                await bus.StartAsync();

                return bus;
            }
        }
    }
}
