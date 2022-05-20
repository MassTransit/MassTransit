namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Sagas;
    using Testing;


    public class Producer_Specs
    {
        const string Topic = "producer";

        [Test]
        public async Task Should_receive_messages()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.Host("localhost:9092");

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            var correlationId = NewId.NextGuid();
            var conversationId = NewId.NextGuid();
            var initiatorId = NewId.NextGuid();
            var messageId = NewId.NextGuid();

            await producer.Produce(new { Text = "text" }, Pipe.Execute<SendContext>(context =>
            {
                context.CorrelationId = correlationId;
                context.MessageId = messageId;
                context.InitiatorId = initiatorId;
                context.ConversationId = conversationId;
                context.Headers.Set("Special", new
                {
                    Key = "Hello",
                    Value = "World"
                });
            }), harness.CancellationToken);

            ConsumeContext<KafkaMessage> result = await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<KafkaMessage>>>().Task;

            Assert.AreEqual("text", result.Message.Text);
            Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
            Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));
            Assert.That(result.MessageId, Is.EqualTo(messageId));
            Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
            Assert.That(result.InitiatorId, Is.EqualTo(initiatorId));
            Assert.That(result.ConversationId, Is.EqualTo(conversationId));

            var headerType = result.Headers.Get<HeaderType>("Special");
            Assert.That(headerType, Is.Not.Null);
            Assert.That(headerType.Key, Is.EqualTo("Hello"));
            Assert.That(headerType.Value, Is.EqualTo("World"));
        }

        [Test]
        public async Task Should_use_bus_send_observer()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.Host("localhost:9092");

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            TaskCompletionSource<SendContext> preSendCompletionSource = harness.GetTask<SendContext>();
            TaskCompletionSource<SendContext> postSendCompletionSource = harness.GetTask<SendContext>();

            harness.Bus.ConnectSendObserver(new TestSendObserver(preSendCompletionSource, postSendCompletionSource));

            ITopicProducer<KafkaMessage> producer = harness.GetProducer<KafkaMessage>();

            await producer.Produce(new { Text = "text" }, harness.CancellationToken);

            await preSendCompletionSource.Task;

            ConsumeContext<KafkaMessage> result = await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<KafkaMessage>>>().Task;

            Assert.AreEqual("text", result.Message.Text);
            Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
            Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));

            await postSendCompletionSource.Task;
        }

        [Test]
        public async Task Should_produce_from_state_machine()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<KafkaMessageConsumer>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.Host("localhost:9092");

                            k.TopicEndpoint<KafkaMessage>(Topic, nameof(Receive_Specs), c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.CreateIfMissing();
                                c.ConfigureConsumer<KafkaMessageConsumer>(context);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var correlationId = NewId.NextGuid();

            await harness.Bus.Publish(new StartTest
            {
                CorrelationId = correlationId,
                TestKey = "ABC"
            }, harness.CancellationToken);

            ConsumeContext<KafkaMessage> result = await provider.GetRequiredService<TaskCompletionSource<ConsumeContext<KafkaMessage>>>().Task;

            Assert.AreEqual("text", result.Message.Text);
            Assert.AreEqual(correlationId, result.InitiatorId);
        }


        class KafkaMessageConsumer :
            IConsumer<KafkaMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<KafkaMessage>> _taskCompletionSource;

            public KafkaMessageConsumer(TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public async Task Consume(ConsumeContext<KafkaMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
            }
        }


        public interface HeaderType
        {
            string Key { get; }
            string Value { get; }
        }


        public interface KafkaMessage
        {
            string Text { get; }
        }


        public class TestInstance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public string Key { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class TestStateMachineSaga :
            MassTransitStateMachine<TestInstance>
        {
            public TestStateMachineSaga()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .Then(context => context.Instance.Key = context.Data.TestKey)
                        .Produce(x => x.Init<KafkaMessage>(new { Text = "text" }))
                        .TransitionTo(Active));

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }

            public Event<StartTest> Started { get; private set; }
        }


        class TestSendObserver :
            ISendObserver
        {
            readonly TaskCompletionSource<SendContext> _postSend;
            readonly TaskCompletionSource<SendContext> _preSend;

            public TestSendObserver(TaskCompletionSource<SendContext> preSend, TaskCompletionSource<SendContext> postSend)
            {
                _preSend = preSend;
                _postSend = postSend;
            }

            public Task PreSend<T>(SendContext<T> context)
                where T : class
            {
                _preSend.TrySetResult(context);
                return Task.CompletedTask;
            }

            public Task PostSend<T>(SendContext<T> context)
                where T : class
            {
                _postSend.TrySetResult(context);
                return Task.CompletedTask;
            }

            public Task SendFault<T>(SendContext<T> context, Exception exception)
                where T : class
            {
                return Task.CompletedTask;
            }
        }
    }
}
