namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;
    using Testing;


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

            await Bus.Publish(message, Pipe.Execute<SendContext>(ctx => ctx.SetGroupId(message.CorrelationId.ToString())));

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
                        configurator.QueueAttributes[QueueAttributeName.ContentBasedDeduplication] = true;
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
                var id = NewId.NextGuid();
                await busControl.Publish(new MessageInOrder(), Pipe.Execute<SendContext>(ctx => ctx.SetGroupId(id.ToString())));

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


    [TestFixture]
    public class When_sending_a_bunch_of_messages_in_the_same_group
    {
        [Test]
        public async Task Should_arrive_in_order()
        {
            await using var provider = new ServiceCollection()
                .AddSingleton<IList<ConsumeContext<MessageInOrder>>, List<ConsumeContext<MessageInOrder>>>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<MessageInOrderConsumer>()
                        .Endpoint(e =>
                        {
                            e.ConfigureConsumeTopology = false;

                            e.Name = "in-order.fifo";
                        });

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

                        cfg.ConfigureEndpoints(context);
                    });
                }).BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var groupId = NewId.NextGuid().ToString();

            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:in-order.fifo"));

            const int limit = 20;

            for (var i = 0; i < limit; i++)
            {
                await endpoint.Send(new MessageInOrder
                {
                    Track = true,
                    Index = i
                }, x =>
                {
                    x.SetGroupId(groupId);
                    x.SetDeduplicationId(x.MessageId.ToString());
                });
            }

            await harness.Consumed.SelectAsync<MessageInOrder>().Take(limit).ToListAsync();

            var results = provider.GetRequiredService<IList<ConsumeContext<MessageInOrder>>>();

            Assert.That(results.Select(x => x.Message.Index), Is.EqualTo(Enumerable.Range(0, limit)));
        }

        [Test]
        public async Task Should_handle_multiple_groups_in_order()
        {
            await using var provider = new ServiceCollection()
                .AddSingleton<IList<ConsumeContext<MessageInOrder>>, List<ConsumeContext<MessageInOrder>>>()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<MessageInOrderConsumer>()
                        .Endpoint(e =>
                        {
                            e.ConfigureConsumeTopology = false;

                            e.Name = "in-order-many.fifo";
                        });

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.LocalstackHost();

                        cfg.ConfigureEndpoints(context);
                    });
                }).BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();


            var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:in-order-many.fifo"));

            const int groupLimit = 5;
            const int limit = 10;

            var groupIds = NewId.NextGuid(groupLimit).Select(x => x.ToString()).ToArray();

            for (var i = 0; i < limit; i++)
            {
                for (var j = 0; j < groupLimit; j++)
                {
                    await endpoint.Send(new MessageInOrder
                    {
                        Track = j == groupLimit - 1,
                        Index = i
                    }, x =>
                    {
                        x.SetGroupId(groupIds[j]);
                        x.SetDeduplicationId(x.MessageId.ToString());
                    });
                }
            }

            await harness.Consumed.SelectAsync<MessageInOrder>().Take(limit * groupLimit).ToListAsync();

            var results = provider.GetRequiredService<IList<ConsumeContext<MessageInOrder>>>();

            Assert.That(results.Select(x => x.Message.Index), Is.EqualTo(Enumerable.Range(0, limit)));
        }


        public class MessageInOrder
        {
            public bool Track { get; set; }
            public int Index { get; set; }
        }


        class MessageInOrderConsumer :
            IConsumer<MessageInOrder>
        {
            readonly ILogger<MessageInOrderConsumer> _logger;
            readonly IList<ConsumeContext<MessageInOrder>> _messages;

            public MessageInOrderConsumer(IList<ConsumeContext<MessageInOrder>> messages, ILogger<MessageInOrderConsumer> logger)
            {
                _messages = messages;
                _logger = logger;
            }

            public async Task Consume(ConsumeContext<MessageInOrder> context)
            {
                await Task.Delay(100);

                if (context.Message.Track)
                {
                    lock (_messages)
                        _messages.Add(context);
                }
            }
        }
    }
}
