namespace MassTransit.Testing
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure.Core.Amqp;
    using Azure.Messaging.ServiceBus;
    using AzureServiceBusTransport;
    using Microsoft.Extensions.DependencyInjection;
    using Serialization;


    public static class AzureFunctionsTestExtensions
    {
        public static IBusRegistrationConfigurator AddAzureFunctionsTestComponents(this IBusRegistrationConfigurator configurator)
        {
            configurator.AddSingleton<IAsyncBusHandle, AsyncBusHandle>()
                .AddSingleton<IMessageReceiver, MessageReceiver>();

            return configurator;
        }

        /// <summary>
        /// Handle the Azure Service Bus message using the specified consumer
        /// </summary>
        /// <param name="harness"></param>
        /// <param name="message"></param>
        /// <typeparam name="TConsumer"></typeparam>
        public static Task HandleConsumer<TConsumer>(this ITestHarness harness, object message)
            where TConsumer : class, IConsumer
        {
            var body = SystemTextJsonMessageSerializer.Instance.SerializeObject(message);

            var messageBody = new AmqpMessageBody(new[] { new BinaryData(body.GetBytes()).ToMemory() });
            var annotatedMessage = new AmqpAnnotatedMessage(messageBody)
            {
                Header = { DeliveryCount = 1 },
                Properties =
                {
                    MessageId = new AmqpMessageId(NewId.NextGuid().ToString()),
                    ContentType = SystemTextJsonRawMessageSerializer.JsonContentType.MediaType
                }
            };

            var receivedMessage = (ServiceBusReceivedMessage)typeof(ServiceBusReceivedMessage).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(AmqpAnnotatedMessage) }, null).Invoke(new object[] { annotatedMessage });

            var receiver = harness.Scope.ServiceProvider.GetRequiredService<IMessageReceiver>();
            var formatter = harness.Scope.ServiceProvider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;

            return receiver.HandleConsumer<TConsumer>(formatter.Consumer<TConsumer>(), receivedMessage, harness.CancellationToken);
        }
    }
}
