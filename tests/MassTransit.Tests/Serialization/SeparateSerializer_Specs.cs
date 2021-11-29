namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class SeparateSerializer_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_both_serializers()
        {
            Task<ConsumeContext<PongMessage>> ponged = await ConnectPublishHandler<PongMessage>();

            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> pingContext = await _handled;

            Assert.That(pingContext.ReceiveContext.ContentType, Is.EqualTo(NewtonsoftJsonMessageSerializer.JsonContentType),
                $"actual ping type is {pingContext.ReceiveContext.ContentType}");

            ConsumeContext<PongMessage> pongContext = await ponged;

            Assert.That(pongContext.ReceiveContext.ContentType, Is.EqualTo(BsonMessageSerializer.BsonContentType),
                $"actual type is {pongContext.ReceiveContext.ContentType}");
        }

        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseBsonDeserializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseBsonSerializer();

            _handled = Handler<PingMessage>(configurator, async context =>
            {
                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            });
        }
    }


    [TestFixture]
    public class Sending_and_consuming_raw_json :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_any_requested_message_type()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Command> context = await _handled;

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(NewtonsoftRawJsonMessageSerializer.RawJsonContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(context.Message.ItemNumber, Is.EqualTo(message.ItemNumber));
        }

        Task<ConsumeContext<Command>> _handled;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRawJsonSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _handled = Handled<Command>(configurator);
        }


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
    public class Sending_and_consuming_raw_xml :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_any_requested_message_type()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27"
            };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Command> context = await _handled;

            Assert.That(context.ReceiveContext.ContentType, Is.EqualTo(RawXmlMessageSerializer.RawXmlContentType),
                $"unexpected content-type {context.ReceiveContext.ContentType}");

            Assert.That(context.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(context.Message.ItemNumber, Is.EqualTo(message.ItemNumber));
        }

        Task<ConsumeContext<Command>> _handled;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseRawXmlSerializer();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _handled = Handled<Command>(configurator);
        }


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
}
