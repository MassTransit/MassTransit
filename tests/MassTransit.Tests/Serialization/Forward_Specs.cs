namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Forwarding_a_json_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_original_headers()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27",
                Crap = "All"
            };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Command> handled = await _handled;

            Assert.That(handled.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(handled.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

            ConsumeContext<BagOfCrap> forwarded = await _forwarded;

            Assert.That(forwarded.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(forwarded.Message.ItemNumber, Is.EqualTo(message.ItemNumber));
            Assert.That(forwarded.Message.Crap, Is.EqualTo("All"));
            Assert.That(forwarded.ReceiveContext.ContentType.MediaType, Is.EqualTo(NewtonsoftJsonMessageSerializer.ContentTypeHeaderValue));
        }

        Task<ConsumeContext<Command>> _handled;
        Task<ConsumeContext<BagOfCrap>> _forwarded;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("forward", x =>
            {
                _forwarded = Handled<BagOfCrap>(x);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _handled = Handler<Command>(configurator, context => context.Forward(new Uri("queue:forward")));
        }


        public interface Command
        {
            Guid CommandId { get; }
            string ItemNumber { get; }
        }


        public class BagOfCrap :
            Command
        {
            public string Crap { get; set; }
            public Guid CommandId { get; set; }
            public string ItemNumber { get; set; }
        }
    }


    [TestFixture]
    public class Forwarding_a_json_message_to_system_text_json :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_original_headers()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27",
                Crap = "All"
            };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Command> handled = await _handled;

            Assert.That(handled.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(handled.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

            ConsumeContext<BagOfCrap> forwarded = await _forwarded;

            Assert.That(forwarded.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(forwarded.Message.ItemNumber, Is.EqualTo(message.ItemNumber));
            Assert.That(forwarded.Message.Crap, Is.EqualTo("All"));
            Assert.That(forwarded.ReceiveContext.ContentType.MediaType, Is.EqualTo(NewtonsoftJsonMessageSerializer.ContentTypeHeaderValue));
        }

        Task<ConsumeContext<Command>> _handled;
        Task<ConsumeContext<BagOfCrap>> _forwarded;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("forward", x =>
            {
                _forwarded = Handled<BagOfCrap>(x);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _handled = Handler<Command>(configurator, context => context.Forward(new Uri("queue:forward")));
        }


        public interface Command
        {
            Guid CommandId { get; }
            string ItemNumber { get; }
        }


        public class BagOfCrap :
            Command
        {
            public string Crap { get; set; }
            public Guid CommandId { get; set; }
            public string ItemNumber { get; set; }
        }
    }


    [TestFixture]
    public class Forwarding_an_xml_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_original_headers()
        {
            var message = new BagOfCrap
            {
                CommandId = NewId.NextGuid(),
                ItemNumber = "27",
                Crap = "All"
            };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Command> handled = await _handled;

            Assert.That(handled.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(handled.Message.ItemNumber, Is.EqualTo(message.ItemNumber));

            ConsumeContext<BagOfCrap> forwarded = await _forwarded;

            Assert.That(forwarded.Message.CommandId, Is.EqualTo(message.CommandId));
            Assert.That(forwarded.Message.ItemNumber, Is.EqualTo(message.ItemNumber));
            Assert.That(forwarded.Message.Crap, Is.EqualTo("All"));
            Assert.That(forwarded.ReceiveContext.ContentType.MediaType, Is.EqualTo(NewtonsoftXmlMessageSerializer.ContentTypeHeaderValue));
        }

        Task<ConsumeContext<Command>> _handled;
        Task<ConsumeContext<BagOfCrap>> _forwarded;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseXmlSerializer();

            configurator.ReceiveEndpoint("forward", x =>
            {
                _forwarded = Handled<BagOfCrap>(x);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _handled = Handler<Command>(configurator, context => context.Forward(new Uri("queue:forward")));
        }


        public interface Command
        {
            Guid CommandId { get; }
            string ItemNumber { get; }
        }


        public class BagOfCrap :
            Command
        {
            public string Crap { get; set; }
            public Guid CommandId { get; set; }
            public string ItemNumber { get; set; }
        }
    }
}
