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
            var consumerConfig = new ConsumerConfig { GroupId = nameof(Producer_Specs) };
            var producerConfig = new ProducerConfig();

            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        r.AddProducer<KafkaMessage>(Topic, producerConfig);
                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, consumerConfig, c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
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

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.Text, Is.EqualTo("text"));
                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));
                Assert.That(result.MessageId, Is.EqualTo(messageId));
                Assert.That(result.CorrelationId, Is.EqualTo(correlationId));
                Assert.That(result.InitiatorId, Is.EqualTo(initiatorId));
                Assert.That(result.ConversationId, Is.EqualTo(conversationId));
            });

            var headerType = result.Headers.Get<HeaderType>("Special");
            Assert.That(headerType, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(headerType.Key, Is.EqualTo("Hello"));
                Assert.That(headerType.Value, Is.EqualTo("World"));
            });
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
    }


    public class Producer_Provider_Specs
    {
        const string Topic = "producer-provider";

        [Test]
        public async Task Should_receive_messages()
        {
            var consumerConfig = new ConsumerConfig { GroupId = nameof(Producer_Provider_Specs) };

            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(15));
                    x.AddRider(r =>
                    {
                        r.UsingKafka((_, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, consumerConfig, c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.Handler<KafkaMessage>(_ => Task.CompletedTask);
                            });
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var producerProvider = harness.Scope.ServiceProvider.GetRequiredService<ITopicProducerProvider>();

            ITopicProducer<KafkaMessage> producer = producerProvider.GetProducer<KafkaMessage>(new Uri($"topic:{Topic}"));

            await producer.Produce(new { }, harness.CancellationToken);

            await harness.Consumed.Any<KafkaMessage>();
        }


        public interface KafkaMessage
        {
        }
    }


    public class ProducerWithObserver_Specs
    {
        const string Topic = "producer-bus-observer";

        [Test]
        public async Task Should_use_bus_send_observer()
        {
            var consumerConfig = new ConsumerConfig { GroupId = nameof(ProducerWithObserver_Specs) };
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, consumerConfig, c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;
                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
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

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.Text, Is.EqualTo("text"));
                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));
            });

            await postSendCompletionSource.Task;
        }


        public interface KafkaMessage
        {
            string Text { get; }
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


    public class ProducerStateMachine_Specs
    {
        const string Topic = "producer-state-machine";

        [Test]
        public async Task Should_produce_from_state_machine()
        {
            var consumerConfig = new ConsumerConfig { GroupId = nameof(ProducerStateMachine_Specs) };
            await using var provider = new ServiceCollection()
                .ConfigureKafkaTestOptions(options =>
                {
                    options.CreateTopicsIfNotExists = true;
                    options.TopicNames = new[] { Topic };
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddTaskCompletionSource<ConsumeContext<KafkaMessage>>();

                    x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>();

                    x.AddRider(r =>
                    {
                        r.AddConsumer<TestKafkaMessageConsumer<KafkaMessage>>();

                        r.AddProducer<KafkaMessage>(Topic);

                        r.UsingKafka((context, k) =>
                        {
                            k.TopicEndpoint<KafkaMessage>(Topic, consumerConfig, c =>
                            {
                                c.AutoOffsetReset = AutoOffsetReset.Earliest;

                                c.ConfigureConsumer<TestKafkaMessageConsumer<KafkaMessage>>(context);
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

            var result = await provider.GetTask<ConsumeContext<KafkaMessage>>();

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.Text, Is.EqualTo("text"));
                Assert.That(result.InitiatorId, Is.EqualTo(correlationId));
            });
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
    }
}
