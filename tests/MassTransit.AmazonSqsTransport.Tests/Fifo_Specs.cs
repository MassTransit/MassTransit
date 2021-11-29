namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Courier;
    using MassTransit.Testing;
    using MassTransit.Topology;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_sending_messages_using_fifo_topics_and_queues :
        AmazonSqsTestFixture
    {
        [Test]
        public async Task Should_allow_it_to_complete()
        {
            var message = new OrderedMessage
            {
                CorrelationId = NewId.NextGuid(),
                Value = "Hello"
            };

            await Bus.Publish(message);

            await AmazonSqsTestHarness.Consumed.Any<PingMessage>(x => x.Context.Message.CorrelationId == message.CorrelationId);

            await _handled;
        }

        Task<ConsumeContext<OrderedMessage>> _handled;

        protected override void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
            configurator.Message<OrderedMessage>(x => x.SetEntityName("ordered-message.fifo"));
            configurator.Publish<OrderedMessage>(x => x.TopicAttributes.Add("ContentBasedDeduplication", "true"));

            configurator.ReceiveEndpoint("ordered-queue.fifo", x =>
            {
                x.QueueAttributes.Add("ContentBasedDeduplication", "true");

                _handled = Handled<OrderedMessage>(x);
            });
        }


        public class OrderedMessage
        {
            public Guid CorrelationId { get; set; }
            public string Value { get; set; }
        }
    }


    [TestFixture]
    public class When_using_an_entity_name_formatter :
        BusTestFixture
    {
        [Test]
        public async Task Should_properly_fifo_the_things()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(LoggerFactory);
            services.AddSingleton(new TaskCompletionSource<ConsumeContext<MessageInOrder>>());
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(new FifoEndpointNameFormatter(new KebabCaseEndpointNameFormatter(false)));

                x.AddConsumer<MessageInOrderConsumer>();

                x.AddConfigureEndpointsCallback((name, _) =>
                {
                    if (_ is IAmazonSqsReceiveEndpointConfigurator configurator)
                    {
                        configurator.QueueAttributes[QueueAttributeName.ContentBasedDeduplication] = true;
                    }
                });

                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.MessageTopology.SetEntityNameFormatter(new FifoEntityNameFormatter());
                    cfg.PublishTopology.TopicAttributes[QueueAttributeName.ContentBasedDeduplication] = true;

                    cfg.Host(new Uri("amazonsqs://localhost:4576"), h =>
                    {
                        h.AccessKey("admin");
                        h.SecretKey("admin");

                        h.Config(new AmazonSQSConfig { ServiceURL = "http://localhost:4566" });
                        h.Config(new AmazonSimpleNotificationServiceConfig { ServiceURL = "http://localhost:4566" });
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            await using var provider = services.BuildServiceProvider(true);

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(TestCancellationToken);
            try
            {
                await busControl.Publish(new MessageInOrder());

                var source = provider.GetRequiredService<TaskCompletionSource<ConsumeContext<MessageInOrder>>>();

                await source.Task;
            }
            finally
            {
                await busControl.StopAsync();
            }
        }

        public When_using_an_entity_name_formatter()
            : base(new InMemoryTestHarness())
        {
        }


        public class MessageInOrder
        {
        }


        class MessageInOrderConsumer :
            IConsumer<MessageInOrder>
        {
            readonly TaskCompletionSource<ConsumeContext<MessageInOrder>> _source;

            public MessageInOrderConsumer(TaskCompletionSource<ConsumeContext<MessageInOrder>> source)
            {
                _source = source;
            }

            public Task Consume(ConsumeContext<MessageInOrder> context)
            {
                _source.TrySetResult(context);

                return Task.CompletedTask;
            }
        }


        class FifoEntityNameFormatter :
            IEntityNameFormatter
        {
            public string FormatEntityName<T>()
            {
                return $"{typeof(T).Name}.fifo";
            }
        }


        class FifoEndpointNameFormatter :
            IEndpointNameFormatter
        {
            readonly IEndpointNameFormatter _formatter;

            public FifoEndpointNameFormatter(IEndpointNameFormatter formatter)
            {
                _formatter = formatter;
            }

            public string Separator => _formatter.Separator;

            public string TemporaryEndpoint(string tag)
            {
                return _formatter.TemporaryEndpoint(tag) + ".fifo";
            }

            public string Consumer<T>()
                where T : class, IConsumer
            {
                return _formatter.Consumer<T>() + ".fifo";
            }

            public string Message<T>()
                where T : class
            {
                return _formatter.Message<T>() + ".fifo";
            }

            public string Saga<T>()
                where T : class, ISaga
            {
                return _formatter.Saga<T>() + ".fifo";
            }

            public string ExecuteActivity<T, TArguments>()
                where T : class, IExecuteActivity<TArguments>
                where TArguments : class
            {
                return _formatter.ExecuteActivity<T, TArguments>() + ".fifo";
            }

            public string CompensateActivity<T, TLog>()
                where T : class, ICompensateActivity<TLog>
                where TLog : class
            {
                return _formatter.CompensateActivity<T, TLog>() + ".fifo";
            }

            public string SanitizeName(string name)
            {
                return _formatter.SanitizeName(name);
            }
        }
    }
}
