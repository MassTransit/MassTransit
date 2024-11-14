namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_correlated_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var pingMessage = new PingMessage();
            await Bus.Publish(pingMessage);

            ConsumeContext<PingMessage> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(pingMessage.CorrelationId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Sending_a_correlated_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var pingMessage = new PingMessage();

            await InputQueueSendEndpoint.Send(pingMessage);

            ConsumeContext<PingMessage> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(pingMessage.CorrelationId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Sending_a_correlation_id_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A { CorrelationId = NewId.NextGuid() };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<A> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(message.CorrelationId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture]
    public class Sending_a_command_id_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A { CommandId = NewId.NextGuid() };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<A> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(message.CommandId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid CommandId { get; set; }
        }
    }


    [TestFixture]
    public class Sending_an_event_id_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A { EventId = NewId.NextGuid() };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<A> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(message.EventId));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid EventId { get; set; }
            public Guid CommandId { get; set; }
        }
    }


    [TestFixture]
    public class Sending_a_nullable_correlation_id_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A { CorrelationId = NewId.NextGuid() };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<A> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(message.CorrelationId.Value));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        class A
        {
            public Guid? CorrelationId { get; set; }
        }
    }


    [TestFixture]
    public class Sending_a_nullable_correlation_id_base_class_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_correlation_id()
        {
            var message = new A { CorrelationId = NewId.NextGuid() };

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<A> context = await _handled;

            Assert.Multiple(() =>
            {
                Assert.That(context.CorrelationId.HasValue, Is.True);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(message.CorrelationId.Value));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<A>(configurator);
        }


        public interface IA
        {
            Guid? CorrelationId { get; }
        }


        class A :
            IA
        {
            public Guid? CorrelationId { get; set; }
        }
    }
}
