namespace MassTransit.KafkaIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using Confluent.Kafka;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;
    using Util;


    public class Producer_Specs :
        InMemoryTestFixture
    {
        const string Topic = "producer";

        [Test]
        public async Task Should_produce()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(Producer_Specs), c =>
                        {
                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                var correlationId = NewId.NextGuid();
                var conversationId = NewId.NextGuid();
                var initiatorId = NewId.NextGuid();
                var messageId = NewId.NextGuid();
                await producer.Produce(new {Text = "text"}, Pipe.Execute<SendContext>(context =>
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
                    }),
                    TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

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
            finally
            {
                serviceScope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }

        [Test]
        public async Task Should_use_bus_send_observer()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            TaskCompletionSource<SendContext> preSendCompletionSource = GetTask<SendContext>();
            TaskCompletionSource<SendContext> postSendCompletionSource = GetTask<SendContext>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ConnectSendObserver(new TestSendObserver(preSendCompletionSource, postSendCompletionSource));
                    cfg.ConfigureEndpoints(context);
                });
                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(Producer_Specs), c =>
                        {
                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var producer = serviceScope.ServiceProvider.GetRequiredService<ITopicProducer<KafkaMessage>>();

            try
            {
                await producer.Produce(new {Text = "text"}, TestCancellationToken);

                await preSendCompletionSource.Task;

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual("text", result.Message.Text);
                Assert.That(result.SourceAddress, Is.EqualTo(new Uri("loopback://localhost/")));
                Assert.That(result.DestinationAddress, Is.EqualTo(new Uri($"loopback://localhost/{KafkaTopicAddress.PathPrefix}/{Topic}")));

                await postSendCompletionSource.Task;
            }
            finally
            {
                serviceScope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
        }

        [Test]
        public async Task Should_produce_from_state_machine()
        {
            TaskCompletionSource<ConsumeContext<KafkaMessage>> taskCompletionSource = GetTask<ConsumeContext<KafkaMessage>>();
            var services = new ServiceCollection();
            services.AddSingleton(taskCompletionSource);

            services.TryAddSingleton<ILoggerFactory>(LoggerFactory);
            services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .InMemoryRepository();

                x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));

                x.AddRider(rider =>
                {
                    rider.AddConsumer<KafkaMessageConsumer>();

                    rider.AddProducer<KafkaMessage>(Topic);

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<KafkaMessage>(Topic, nameof(Producer_Specs), c =>
                        {
                            c.AutoOffsetReset = AutoOffsetReset.Earliest;
                            c.ConfigureConsumer<KafkaMessageConsumer>(context);
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);

            var serviceScope = provider.CreateScope();

            var publishEndpoint = serviceScope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            try
            {
                var correlationId = NewId.NextGuid();

                await publishEndpoint.Publish(new StartTest
                {
                    CorrelationId = correlationId,
                    TestKey = "ABC"
                }, TestCancellationToken);

                ConsumeContext<KafkaMessage> result = await taskCompletionSource.Task;

                Assert.AreEqual("text", result.Message.Text);
                Assert.AreEqual(correlationId, result.InitiatorId);
            }
            finally
            {
                serviceScope.Dispose();

                await busControl.StopAsync(TestCancellationToken);

                await provider.DisposeAsync();
            }
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
                        .Produce(x => x.Init<KafkaMessage>(new {Text = "text"}))
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
                return TaskUtil.Completed;
            }

            public Task PostSend<T>(SendContext<T> context)
                where T : class
            {
                _postSend.TrySetResult(context);
                return TaskUtil.Completed;
            }

            public Task SendFault<T>(SendContext<T> context, Exception exception)
                where T : class
            {
                return TaskUtil.Completed;
            }
        }
    }
}
