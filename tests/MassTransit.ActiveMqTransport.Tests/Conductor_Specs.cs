namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Definition;
    using NUnit.Framework;


    namespace ConductorTests
    {
        using Contracts;


        namespace Contracts
        {
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
            ActiveMqTestFixture
        {
            [Test]
            public async Task Should_connect_using_the_service_client()
            {
                var serviceClient = Bus.CreateServiceClient();

                var requestClient = serviceClient.CreateRequestClient<DeployPayload>();

                var response = await requestClient.GetResponse<PayloadDeployed>(new {Target = "Bogey"});

                Assert.That(response.Message.Target, Is.EqualTo("Bogey"));
            }

            protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
            {
                configurator.ServiceInstance(instance =>
                {
                    var serviceEndpointName = KebabCaseEndpointNameFormatter.Instance.Consumer<DeployPayloadConsumer>();

                    instance.ReceiveEndpoint(serviceEndpointName, x =>
                    {
                        x.Consumer<DeployPayloadConsumer>();
                    });
                });
            }
        }
    }
}
