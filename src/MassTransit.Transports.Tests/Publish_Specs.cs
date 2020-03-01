namespace MassTransit.Transports.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;


    namespace Publishing
    {
        using GreenPipes.Internals.Extensions;
        using Observers;
        using Serialization;


        public interface TestEvent
        {
            Guid CorrelationId { get; }
        }


        public class Publishing_a_message :
            TransportTest
        {
            [Test]
            public async Task Should_be_handled_by_the_consumer()
            {
                Assert.That(_consumer.Consumed.Select<TestEvent>().Any(), Is.True);

                IReceivedMessage<TestEvent> message = _consumer.Consumed.Select<TestEvent>().First();

                Assert.That(message.Context.CorrelationId, Is.EqualTo(_correlationId));
            }

            [Test]
            public async Task Should_have_the_content_type()
            {
                Assert.That(_consumer.Consumed.Select<TestEvent>().Any(), Is.True);

                IReceivedMessage<TestEvent> message = _consumer.Consumed.Select<TestEvent>().First();

                Assert.That(message.Context.ReceiveContext.ContentType, Is.EqualTo(JsonMessageSerializer.JsonContentType));
            }

            [Test]
            public async Task Should_have_the_content_type_transport_header()
            {
                Assert.That(_consumer.Consumed.Select<TestEvent>().Any(), Is.True);

                IReceivedMessage<TestEvent> message = _consumer.Consumed.Select<TestEvent>().First();

                Assert.That(message.Context.ReceiveContext.TransportHeaders.Get("Content-Type", default(string)),
                    Is.EqualTo(JsonMessageSerializer.ContentTypeHeaderValue));
            }

            [Test]
            public async Task Should_call_the_pre_publish_observer()
            {
                await _publish.PrePublished;
            }

            [Test]
            public async Task Should_call_the_post_publish_observer()
            {
                await _publish.PostPublished;
            }

            [Test]
            public void Should_not_call_the_send_observer()
            {
                Assert.That(async () => await _send.PreSent.OrTimeout(s:5), Throws.TypeOf<TimeoutException>());
            }

            ConsumerTestHarness<TestCommandConsumer> _consumer;
            PublishObserver _publish;
            SendObserver _send;
            Guid _correlationId;

            public Publishing_a_message(Type harnessType)
                : base(harnessType)
            {
            }

            [OneTimeSetUp]
            public async Task Setup()
            {
                _consumer = Harness.Consumer<TestCommandConsumer>();

                await Harness.Start();

                _publish = new PublishObserver(Harness);
                Harness.Bus.ConnectPublishObserver(_publish);

                _send = new SendObserver(Harness);
                Harness.Bus.ConnectSendObserver(_send);

                _correlationId = NewId.NextGuid();

                await Harness.Bus.Publish<TestEvent>(new
                {
                    CorrelationId = _correlationId
                });
            }

            [OneTimeTearDown]
            public Task Stop()
            {
                return Harness.Stop();
            }


            class TestCommandConsumer :
                IConsumer<TestEvent>
            {
                public Task Consume(ConsumeContext<TestEvent> context)
                {
                    return Task.CompletedTask;
                }
            }
        }
    }
}
