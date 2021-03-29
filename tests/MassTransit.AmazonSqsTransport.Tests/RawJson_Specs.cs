namespace MassTransit.AmazonSqsTransport.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RawMessages;
    using Serialization;
    using TestFramework.Messages;


    namespace RawMessages
    {
        using System;


        public interface Command
        {
            Guid CommandId { get; }
            string ItemNumber { get; }
        }


        public class BagOfCrap
        {
            public Guid CommandId { get; set; }
            public string ItemNumber { get; set; }
        }
    }


    [TestFixture]
    public class Sending_and_consuming_raw_json_with_headers :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_return_the_header_value_from_the_transport()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            const string headerName = "Random-Header";
            const string headerValue = "SomeValue";

            await InputQueueSendEndpoint.Send(message, x => x.Headers.Set(headerName, headerValue));

            ConsumeContext<Command> context = await _handled;

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(RawJsonMessageSerializer.RawJsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(context.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

            Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(headerValue));

            Assert.IsTrue(context.MessageId.HasValue);
            Assert.IsTrue(context.ConversationId.HasValue);
            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.IsTrue(context.SentTime.HasValue);
            Assert.IsNotNull(context.DestinationAddress);
            Assert.That(context.SupportedMessageTypes.Count(), Is.EqualTo(1));
        }

        Task<ConsumeContext<Command>> _handled;

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.UseRawJsonSerializer();
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<Command>(configurator);
        }
    }


    [TestFixture]
    public class Sending_and_consuming_raw_json_with_headers_and_producing :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_not_forward_transport_headers_from_raw_json()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            const string headerName = "Random-Header";
            const string headerValue = "SomeValue";

            await InputQueueSendEndpoint.Send(message, x =>
            {
                x.Headers.Set(headerName, headerValue);
                x.Serializer = new RawJsonMessageSerializer();
            });

            ConsumeContext<Command> commandContext = await _handler;

            Assert.That(commandContext.ReceiveContext.ContentType, Is.EqualTo(RawJsonMessageSerializer.RawJsonContentType),
                $"unexpected content-type {commandContext.ReceiveContext.ContentType}");

            ConsumeContext<PingMessage> context = await _handled;

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(JsonMessageSerializer.JsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CorrelationId, Is.EqualTo(message.CommandId));

            Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(default));
        }

        Task<ConsumeContext<PingMessage>> _handled;
        Task<ConsumeContext<Command>> _handler;

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("second-queue", e =>
            {
                _handled = Handled<PingMessage>(e);
            });
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            configurator.AddMessageDeserializer(RawJsonMessageSerializer.RawJsonContentType,
                () => new RawJsonMessageDeserializer(RawJsonMessageSerializer.Deserializer));

            TaskCompletionSource<ConsumeContext<Command>> handler = GetTask<ConsumeContext<Command>>();
            _handler = handler.Task;

            Handler<Command>(configurator, async context =>
            {
                await context.Publish(new PingMessage(context.Message.CommandId));

                handler.SetResult(context);
            });
        }
    }


    [TestFixture]
    public class Sending_and_consuming_raw_json_with_headers_and_producing_with_copy_enabled :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_not_forward_transport_headers_from_raw_json()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            const string headerName = "Random-Header";
            const string headerValue = "SomeValue";

            await InputQueueSendEndpoint.Send(message, x =>
            {
                x.Headers.Set(headerName, headerValue);
                x.Serializer = new RawJsonMessageSerializer();
            });

            ConsumeContext<Command> commandContext = await _handler;

            Assert.That(commandContext.ReceiveContext.ContentType, Is.EqualTo(RawJsonMessageSerializer.RawJsonContentType),
                $"unexpected content-type {commandContext.ReceiveContext.ContentType}");

            ConsumeContext<PingMessage> context = await _handled;

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(JsonMessageSerializer.JsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CorrelationId, Is.EqualTo(message.CommandId));

            Assert.That(context.Headers.Get<string>(headerName), Is.EqualTo(headerValue));
        }

        Task<ConsumeContext<PingMessage>> _handled;
        Task<ConsumeContext<Command>> _handler;

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("second-queue", e =>
            {
                _handled = Handled<PingMessage>(e);
            });
        }

        protected override void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
            configurator.AddMessageDeserializer(RawJsonMessageSerializer.RawJsonContentType,
                () => new RawJsonMessageDeserializer(RawJsonMessageSerializer.Deserializer, RawJsonSerializerOptions.All));

            TaskCompletionSource<ConsumeContext<Command>> handler = GetTask<ConsumeContext<Command>>();
            _handler = handler.Task;

            Handler<Command>(configurator, async context =>
            {
                await context.Publish(new PingMessage(context.Message.CommandId));

                handler.SetResult(context);
            });
        }
    }
}
