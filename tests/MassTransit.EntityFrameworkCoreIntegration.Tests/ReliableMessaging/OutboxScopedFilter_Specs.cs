namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Threading.Tasks;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;
    using Util;


    public class OutboxScopedFilter_Specs
    {
        [Test]
        public async Task Should_call_the_scoped_filter_on_publish()
        {
            TaskCompletionSource<MyId> taskCompletionSource = TaskUtil.GetTask<MyId>();

            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddScoped(_ => new MyId(Guid.NewGuid()))
                .AddSingleton(taskCompletionSource)
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<SimplerConsumer, SimplerConsumerDefinition>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishFilter(typeof(TestScopedPublishFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            IConsumerTestHarness<SimplerConsumer> consumerHarness = harness.GetConsumerHarness<SimplerConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    await publishEndpoint.Publish<SimpleMessageInterface>(new { Name = "Frank" });

                    await dbContext.SaveChangesAsync(harness.CancellationToken);
                }

                Assert.That(await consumerHarness.Consumed.Any<SimpleMessageInterface>(), Is.True);

                await taskCompletionSource.Task;
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_call_the_scoped_filter_on_send()
        {
            TaskCompletionSource<MyId> taskCompletionSource = TaskUtil.GetTask<MyId>();

            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddScoped(_ => new MyId(Guid.NewGuid()))
                .AddSingleton(taskCompletionSource)
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<SimplerConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseSendFilter(typeof(TestScopedSendFilter<>), context);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            IConsumerTestHarness<SimplerConsumer> consumerHarness = harness.GetConsumerHarness<SimplerConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var sendEndpointProvider = harness.Scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>();

                    var formatter = provider.GetService<IEndpointNameFormatter>() ?? DefaultEndpointNameFormatter.Instance;

                    var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:{formatter.Consumer<SimplerConsumer>()}"));

                    await endpoint.Send<SimpleMessageInterface>(new { Name = "Frank" });

                    await dbContext.SaveChangesAsync(harness.CancellationToken);
                }

                Assert.That(await consumerHarness.Consumed.Any<SimpleMessageInterface>(), Is.True);

                var myId = harness.Scope.ServiceProvider.GetRequiredService<MyId>();

                var result = await taskCompletionSource.Task;
                Assert.That(result, Is.EqualTo(myId));
            }
            finally
            {
                await harness.Stop();
            }
        }


        public class MyId :
            IEquatable<MyId>
        {
            readonly Guid _id;

            public MyId(Guid id)
            {
                _id = id;
            }

            public bool Equals(MyId other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return _id.Equals(other._id);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((MyId)obj);
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }

            public override string ToString()
            {
                return _id.ToString();
            }
        }


        public interface SimpleMessageInterface
        {
            string Name { get; }
        }


        public class SimplerConsumer :
            IConsumer<SimpleMessageInterface>
        {
            static readonly TaskCompletionSource<SimplerConsumer> _consumerCreated = TaskUtil.GetTask<SimplerConsumer>();

            readonly TaskCompletionSource<SimpleMessageInterface> _received;

            public SimplerConsumer()
            {
                _received = TaskUtil.GetTask<SimpleMessageInterface>();

                _consumerCreated.TrySetResult(this);
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> message)
            {
                await message.Publish<SimpleEvent>(new { });

                _received.TrySetResult(message.Message);
            }
        }


        class SimplerConsumerDefinition :
            ConsumerDefinition<SimplerConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<SimplerConsumer> consumerConfigurator, IRegistrationContext context)
            {
                endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(context);
            }
        }


        public interface SimpleEvent
        {
            string Name { get; }
        }


        class TestScopedPublishFilter<T> :
            IFilter<PublishContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly IServiceProvider _provider;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public TestScopedPublishFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId, IServiceProvider provider)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
                _provider = provider;
            }

            public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
            {
                if (context is PublishContext<SimpleEvent> && _provider.GetService<ScopedConsumeContextProvider>()?.HasContext == true)
                    _taskCompletionSource.TrySetResult(_myId);

                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class TestScopedSendFilter<T> :
            IFilter<SendContext<T>>
            where T : class
        {
            readonly MyId _myId;
            readonly TaskCompletionSource<MyId> _taskCompletionSource;

            public TestScopedSendFilter(TaskCompletionSource<MyId> taskCompletionSource, MyId myId)
            {
                _taskCompletionSource = taskCompletionSource;
                _myId = myId;
            }

            public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
            {
                _taskCompletionSource.TrySetResult(_myId);
                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
