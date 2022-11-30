namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using AzureServiceBusTransport;
    using FunctionComponents;
    using global::Azure.Core.Amqp;
    using global::Azure.Messaging.ServiceBus;
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
                .AddSingleton<IAsyncBusHandle, AsyncBusHandle>()
                .AddSingleton<IMessageReceiver, MessageReceiver>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<FaultyFunctionConsumer, FaultyFunctionConsumerDefinition>();
                    x.UsingTestAzureServiceBus((context, cfg) =>
                    {
                        cfg.UseRawJsonDeserializer(isDefault: true);
                    }, false);
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            var receiver = provider.GetRequiredService<IMessageReceiver>();

            var messageBody = new AmqpMessageBody(new[] { new BinaryData("{}").ToMemory() });
            var annotatedMessage = new AmqpAnnotatedMessage(messageBody);
            annotatedMessage.Header.DeliveryCount = 1;
            annotatedMessage.Properties.MessageId = new AmqpMessageId(NewId.NextGuid().ToString());
            annotatedMessage.Properties.ContentType = "application/json";

            var message = (ServiceBusReceivedMessage)typeof(ServiceBusReceivedMessage).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(AmqpAnnotatedMessage) }, null).Invoke(new object[] { annotatedMessage });

            Assert.That(async () => await receiver.HandleConsumer<FaultyFunctionConsumer>("input-queue", message, harness.CancellationToken),
                Throws.TypeOf<IntentionalTestException>());

            IConsumerTestHarness<FaultyFunctionConsumer> consumerHarness = harness.GetConsumerHarness<FaultyFunctionConsumer>();

            Assert.That(await consumerHarness.Consumed.Any<FunctionMessage>(), Is.True);

            Assert.That(await harness.Published.Any<Fault<FunctionMessage>>(), Is.True);
        }
    }


    namespace FunctionComponents
    {
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
                IConsumerConfigurator<FaultyFunctionConsumer> consumerConfigurator)
            {
                endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMilliseconds(1)));
            }
        }
    }
}
