namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            var message = new PingMessage();
            await Bus.Publish(message);

            await _received;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_object :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            object message = new PingMessage();
            await Bus.Publish(message);

            await _received;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_a_message_with_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            var message = new PingMessage();
            await Bus.Publish(message, Pipe.New<PublishContext>(x => x.UseExecute(v => v.RequestId = _requestId)));

            ConsumeContext<PingMessage> context = await _received;

            Assert.Multiple(() =>
            {
                Assert.That(context.RequestId.HasValue, Is.True);
                Assert.That(context.RequestId.Value, Is.EqualTo(_requestId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_a_message_with_context_type :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            var message = new PingMessage();
            await Bus.Publish(message, Pipe.New<PublishContext<PingMessage>>(x => x.UseExecute(v => v.RequestId = _requestId)));

            ConsumeContext<PingMessage> context = await _received;

            Assert.Multiple(() =>
            {
                Assert.That(context.RequestId.HasValue, Is.True);
                Assert.That(context.RequestId.Value, Is.EqualTo(_requestId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_object_with_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            object message = new PingMessage();
            await Bus.Publish(message, context => context.RequestId = _requestId);

            ConsumeContext<PingMessage> consumeContext = await _received;

            Assert.That(consumeContext.RequestId, Is.EqualTo(_requestId));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_interface_with_object :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            await Bus.Publish<PingMessage>(new { });

            ConsumeContext<PingMessage> context = await _received;
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_interface_with_object_and_context :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_properly()
        {
            await Bus.Publish<PingMessage>(new { }, v => v.RequestId = _requestId);

            ConsumeContext<PingMessage> context = await _received;

            Assert.Multiple(() =>
            {
                Assert.That(context.RequestId.HasValue, Is.True);
                Assert.That(context.RequestId, Is.EqualTo(_requestId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _received;
        #pragma warning restore NUnit1032
        readonly Guid _requestId = NewId.NextGuid();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<PingMessage>(configurator);
        }
    }
}
