namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    namespace Send_Specs
    {
        using System;
        using System.Linq;
        using System.Threading.Tasks;
        using MassTransit.Testing;
        using NUnit.Framework;
        using RabbitMQ.Client;
        using Serialization;
        using Shouldly;
        using TestFramework;


        [TestFixture]
        public class WhenAMessageIsSendToTheEndpoint :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                var endpoint = await Bus.GetSendEndpoint(InputQueueAddress);

                var message = new A { Id = Guid.NewGuid() };
                await endpoint.Send(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handled<A>(configurator);
            }
        }

        [TestFixture]
        public class WhenAMessageIsSendToTheEndpointWithAGuidHeader :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                var endpoint = await Bus.GetSendEndpoint(InputQueueAddress);

                var message = new A { Id = Guid.NewGuid() };
                await endpoint.Send(message, context =>
                {
                    Guid? value = NewId.NextGuid();
                    context.Headers.Set(MessageHeaders.SchedulingTokenId, value);
                });

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handled<A>(configurator);
            }
        }


        [TestFixture]
        public class When_a_message_is_send_to_the_bus_itself :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                Task<ConsumeContext<A>> receivedA = SubscribeHandler<A>();

                var message = new A { Id = Guid.NewGuid() };
                await BusSendEndpoint.Send(message);

                ConsumeContext<A> received = await receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }
        }


        [TestFixture]
        public class WhenAMessageIsSendToTheEndpointEncrypted :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                var endpoint = await Bus.GetSendEndpoint(InputQueueAddress);

                var message = new A { Id = Guid.NewGuid() };
                await endpoint.Send(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);

                Assert.AreEqual(EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handled<A>(configurator);
            }

            protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
            {
                ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider();
                var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");
                configurator.UseEncryptedSerializer(streamProvider);

                base.ConfigureRabbitMqBus(configurator);
            }
        }


        [TestFixture]
        public class WhenAMessageIsPublishedToTheEndpoint :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                var message = new A { Id = Guid.NewGuid() };
                await Bus.Publish(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handled<A>(configurator);
            }
        }


        [TestFixture]
        [Explicit]
        public class WhenAMessageIsPublishedToTheEndpointSuccessfully :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_not_increase_channel_count()
            {
                var message = new A { Id = Guid.NewGuid() };
                await Bus.Publish(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);
            }

            [Test]
            public async Task Should_take_time_to_watch_channel_use()
            {
                ConsumeContext<A> received = await _receivedA;

                await Task.Delay(15000);
            }

            Task<ConsumeContext<A>> _receivedA;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handled<A>(configurator);
            }
        }


        [TestFixture]
        [Explicit]
        public class WhenAMessageIsPublishedToTheEndpointFaulting :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_not_increase_channel_count()
            {
                var message = new A { Id = Guid.NewGuid() };
                await Bus.Publish(message);

                ConsumeContext<Fault<A>> received = await _faultA;

                Assert.AreEqual(message.Id, received.Message.Message.Id);
            }

            [Test]
            public async Task Should_take_time_to_watch_channel_use()
            {
                ConsumeContext<Fault<A>> received = await _faultA;

                await Task.Delay(15000);
            }

            Task<ConsumeContext<A>> _receivedA;
            Task<ConsumeContext<Fault<A>>> _faultA;

            protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
            {
                configurator.ReceiveEndpoint("handle-fault", x =>
                {
                    _faultA = Handled<Fault<A>>(x);
                });
            }

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handler<A>(configurator, context =>
                {
                    throw new IntentionalTestException();
                });
            }

            protected override void OnCleanupVirtualHost(IModel model)
            {
                model.ExchangeDelete("handle-fault");
                model.QueueDelete("handle-fault");
            }
        }


        [TestFixture]
        public class WhenAMessageIsPublishedToATemporaryEndpoint :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                var message = new A { Id = Guid.NewGuid() };

                await Bus.Publish(message);

                await _receivedA;
                await _temporaryA;
                await _temporaryB;
            }

            Task<ConsumeContext<A>> _receivedA;
            Task<ConsumeContext<A>> _temporaryA;
            Task<ConsumeContext<A>> _temporaryB;

            protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
            {
                configurator.ReceiveEndpoint(x =>
                {
                    _temporaryA = Handled<A>(x);
                });

                configurator.ReceiveEndpoint(x =>
                {
                    _temporaryB = Handled<A>(x);
                });
            }

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _receivedA = Handled<A>(configurator);
            }
        }


        [TestFixture]
        public class When_a_message_is_published_from_the_queue :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_have_the_receive_endpoint_input_address()
            {
                var message = new A { Id = Guid.NewGuid() };
                await Bus.Publish(message);

                ConsumeContext<A> received = await _receivedA;

                Assert.AreEqual(message.Id, received.Message.Id);

                ConsumeContext<GotA> consumeContext = await _receivedGotA;

                consumeContext.SourceAddress.ShouldBe(new Uri("rabbitmq://localhost/test/input_queue"));

                Assert.That(consumeContext.ReceiveContext.TransportHeaders.Get(MessageHeaders.MessageId, "N/A"),
                    Is.EqualTo(consumeContext.MessageId.ToString()));
            }

            Task<ConsumeContext<A>> _receivedA;
            Task<ConsumeContext<GotA>> _receivedGotA;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                configurator.PrefetchCount = 16;

                _receivedA = Handler<A>(configurator, context => context.Publish(new GotA { Id = context.Message.Id }));
            }

            protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
            {
                configurator.ReceiveEndpoint("ack_queue", x =>
                {
                    _receivedGotA = Handled<GotA>(x);

                    x.ConsumerPriority = 10;
                });
            }

            protected override void OnCleanupVirtualHost(IModel model)
            {
                model.ExchangeDelete("ack_queue");
                model.QueueDelete("ack_queue");
            }
        }


        [TestFixture]
        public class WhenAMessageIsPublishedToTheConsumer :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_be_received()
            {
                var message = new B { Id = Guid.NewGuid() };

                await Bus.Publish(message);

                _consumer.Received.Select<B>().Any().ShouldBe(true);

                IReceivedMessage<B> receivedMessage = _consumer.Received.Select<B>().First();

                Assert.AreEqual(message.Id, receivedMessage.Context.Message.Id);
            }

            MultiTestConsumer _consumer;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                _consumer = new MultiTestConsumer(TestTimeout);
                _consumer.Consume<B>();

                _consumer.Configure(configurator);
            }
        }


        [TestFixture]
        public class When_a_message_is_published_without_a_queue_binding :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_not_throw_an_exception()
            {
                var message = new UnboundMessage { Id = Guid.NewGuid() };

                await Bus.Publish(message);
            }


            class UnboundMessage
            {
                public Guid Id { get; set; }
            }
        }


        [TestFixture]
        public class When_a_message_is_sent_with_no_subscriber :
            RabbitMqTestFixture
        {
            [Test]
            public async Task Should_not_throw_an_exception()
            {
                var message = new UnboundMessage { Id = Guid.NewGuid() };

                await InputQueueSendEndpoint.Send(message);

                await InputQueueSendEndpoint.Send(new B());

                _consumer.Received.Select<B>().Any().ShouldBe(true);
            }

            MultiTestConsumer _consumer;

            protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                configurator.ConfigureConsumeTopology = false;

                base.ConfigureRabbitMqReceiveEndpoint(configurator);

                configurator.UseConcurrencyLimit(1);

                _consumer = new MultiTestConsumer(TestTimeout);
                _consumer.Consume<B>();

                _consumer.Configure(configurator);
            }


            class UnboundMessage
            {
                public Guid Id { get; set; }
            }
        }


        public class A
        {
            public Guid Id { get; set; }
        }


        class GotA
        {
            public Guid Id { get; set; }
        }


        class B : IEquatable<B>
        {
            public Guid Id { get; set; }

            public bool Equals(B other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Id.Equals(other.Id);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((B)obj);
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }
        }
    }


    [TestFixture]
    public class When_publishing_an_interface_message :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_have_correlation_id()
        {
            await InputQueueSendEndpoint.Send<IProxyMe>(new
            {
                IntValue,
                StringValue,
                CorrelationId = _correlationId
            });

            ConsumeContext<IProxyMe> message = await _handler;

            message.Message.CorrelationId.ShouldBe(_correlationId);
            message.Message.IntValue.ShouldBe(IntValue);
            message.Message.StringValue.ShouldBe(StringValue);
        }

        const int IntValue = 42;
        const string StringValue = "Hello";
        readonly Guid _correlationId = Guid.NewGuid();
        Task<ConsumeContext<IProxyMe>> _handler;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumeTopology = false;

            _handler = Handled<IProxyMe>(configurator);
        }


        public interface IProxyMe :
            CorrelatedBy<Guid>
        {
            int IntValue { get; }
            string StringValue { get; }
        }
    }
}
