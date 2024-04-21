namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using FunctionComponents;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class When_a_consumer_faults_in_a_function_receiver
    {
        [Test]
        public async Task Should_retry_if_configured()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddAzureFunctionsTestComponents();

                    x.AddConsumer<FaultyFunctionConsumer, FaultyFunctionConsumerDefinition>();

                    x.UsingTestAzureServiceBus((context, cfg) =>
                    {
                        cfg.UseRawJsonDeserializer(isDefault: true);
                    }, false);
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            Assert.That(async () => await harness.HandleConsumer<FaultyFunctionConsumer>(new FunctionMessage()),
                Throws.TypeOf<IntentionalTestException>());

            IConsumerTestHarness<FaultyFunctionConsumer> consumerHarness = harness.GetConsumerHarness<FaultyFunctionConsumer>();

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await consumerHarness.Consumed.Any<FunctionMessage>(), Is.True);

                Assert.That(await harness.Published.Any<Fault<FunctionMessage>>(), Is.True);
            });
        }
    }


    namespace FunctionComponents
    {
        using System;


        public class FunctionMessage
        {
        }


        public class FaultyFunctionConsumer :
            IConsumer<FunctionMessage>
        {
            public Task Consume(ConsumeContext<FunctionMessage> context)
            {
                throw new IntentionalTestException();
            }
        }


        public class FaultyFunctionConsumerDefinition :
            ConsumerDefinition<FaultyFunctionConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<FaultyFunctionConsumer> consumerConfigurator,
                IRegistrationContext context)
            {
                endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMilliseconds(1)));
            }
        }
    }
}
