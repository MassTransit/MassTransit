namespace MassTransit.WebJobs.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Contexts;
    using Context;
    using GreenPipes;
    using Metadata;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Serialization;
    using ServiceBusIntegration;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_brokered_message_receiver
    {
        [Test]
        public async Task Should_create_the_brokered_message_receiver()
        {
            var message = new Mock<Message>();
            message.Object.ContentType = JsonMessageSerializer.ContentTypeHeaderValue;
            message.Object.MessageId = NewId.NextGuid().ToString();

            using (var stream = new MemoryStream())
            {
                ServiceBusSendContext<PingMessage> context = new AzureServiceBusSendContext<PingMessage>(new PingMessage(), CancellationToken.None);

                var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<PingMessage>.MessageTypeNames);

                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.Formatting = Formatting.Indented;

                    JsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                    jsonWriter.Flush();
                    writer.Flush();
                }

                message.Object.Body = stream.ToArray();
            }

            var binder = new Mock<IBinder>();

            LogContext.ConfigureCurrentLogContext();

            var handler = Bus.Factory.CreateBrokeredMessageReceiver(binder.Object, LogContext.Current.Logger, cfg =>
            {
                cfg.InputAddress = new Uri("sb://masstransit-build.servicebus.windows.net/input-queue");

                cfg.UseRetry(x => x.Intervals(10, 100, 500, 1000));
                cfg.Consumer(() => new Consumer());
            });

            Console.WriteLine(handler.GetProbeResult().ToJsonString());

            //            await handler.Handle(message.Object);
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                TestContext.Out.WriteLine("Hello");

                return Task.CompletedTask;
            }
        }
    }
}
