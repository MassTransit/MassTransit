namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;
    using Testing;


    public class Using_the_bus_outbox
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var activity = TraceConfig.Source.StartActivity(ActivityKind.Client);

                    await publishEndpoint.Publish(new PingMessage());

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                    Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    activity.Stop();
                }

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);

                IReceivedMessage<PingMessage> context = harness.Consumed.Select<PingMessage>().Single();

                Assert.Multiple(() =>
                {
                    Assert.That(context.Context.MessageId, Is.Not.Null);
                    Assert.That(context.Context.ConversationId, Is.Not.Null);
                    Assert.That(context.Context.DestinationAddress, Is.Not.Null);
                    Assert.That(context.Context.SourceAddress, Is.Not.Null);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_include_headers_when_using_raw_json()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseRawJsonSerializer();
                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var activity = TraceConfig.Source.StartActivity(ActivityKind.Client);

                    await publishEndpoint.Publish(new PingMessage(), x => x.Headers.Set("Test-Header", "Test-Value"));

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    activity.Stop();
                }

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);

                IReceivedMessage<PingMessage> context = await consumerHarness.Consumed.SelectAsync<PingMessage>().FirstOrDefault();

                Assert.Multiple(() =>
                {
                    Assert.That(context.Context.Headers.TryGetHeader("Test-Header", out var header), Is.True);

                    Assert.That(header, Is.EqualTo("Test-Value"));

                    Assert.That(context.Context.MessageId, Is.Not.Null);
                    Assert.That(context.Context.ConversationId, Is.Not.Null);
                    Assert.That(context.Context.DestinationAddress, Is.Not.Null);
                    Assert.That(context.Context.SourceAddress, Is.Not.Null);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_support_baggage_in_telemetry()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
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

                    x.AddTaskCompletionSource<string>();
                    x.AddConsumer<PingBaggageConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            IConsumerTestHarness<PingBaggageConsumer> consumerHarness = harness.GetConsumerHarness<PingBaggageConsumer>();

            try
            {
                {
                    await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var activity = TraceConfig.Source.StartActivity(ActivityKind.Client);

                    activity.AddBaggage("Suitcase", "Full of cash");

                    await publishEndpoint.Publish(new PingMessage());

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    activity.Stop();
                }

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);

                var source = provider.GetRequiredService<TaskCompletionSource<string>>();
                Assert.That(await source.Task, Is.EqualTo("Full of cash"));
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_support_multiple_save_changes()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            try
            {
                {
                    await using var scope = harness.Scope.ServiceProvider.CreateAsyncScope();
                    await using var dbContext = scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var activity = TraceConfig.Source.StartActivity(ActivityKind.Client);

                    await publishEndpoint.Publish(new PingMessage());

                    using var cts1 = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                    Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts1.Token), Is.False);

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    await publishEndpoint.Publish(new PingMessage());

                    using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                    Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts2.Token), Is.True);

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    activity.Stop();
                }

                Assert.That(consumerHarness.Consumed.Count(harness.CancellationToken), Is.EqualTo(2));
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_start_immediately_when_context_disposed()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromMinutes(10);

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            try
            {
                {
                    await using var scope = harness.Scope.ServiceProvider.CreateAsyncScope();
                    await using var dbContext = scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var activity = TraceConfig.Source.StartActivity(ActivityKind.Client);

                    await publishEndpoint.Publish(new PingMessage());

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                    Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);

                    await dbContext.SaveChangesAsync(harness.CancellationToken);

                    activity.Stop();
                }


                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        [Explicit]
        public async Task Fill_up_the_outbox()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);
                        o.DisableInboxCleanupService();

                        //                        o.UsePostgres();

                        o.UseBusOutbox(bo =>
                        {
                            bo.MessageDeliveryLimit = 10;
                        });
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            await harness.Start();

            var totalTimer = Stopwatch.StartNew();
            var sendTimer = Stopwatch.StartNew();

            const int loopCount = 400;
            const int messagesPerLoop = 3;
            await Task.WhenAll(Enumerable.Range(0, loopCount).Select(async n =>
            {
                // ReSharper disable once AccessToDisposedClosure
                using var scope = provider.CreateScope();

                await using var dbContext = scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await Task.WhenAll(Enumerable.Range(0, messagesPerLoop).Select(_ => publishEndpoint.Publish(new PingMessage())));

                await dbContext.SaveChangesAsync(harness.CancellationToken);
            }));

            sendTimer.Stop();

            var count = await harness.Consumed.SelectAsync<PingMessage>().Count();

            totalTimer.Stop();

            var totalTime = totalTimer.Elapsed - harness.TestInactivityTimeout;

            const int messageCount = loopCount * messagesPerLoop;

            Assert.That(count, Is.EqualTo(messageCount));

            TestContext.Out.WriteLine("Message Count: {0}", messageCount);

            TestContext.Out.WriteLine("Total send duration: {0:g}", sendTimer.Elapsed);
            TestContext.Out.WriteLine("Send message rate: {0:F2} (msg/s)", messageCount * 1000 / sendTimer.Elapsed.TotalMilliseconds);
            TestContext.Out.WriteLine("Total consume duration: {0:g}", totalTime);
            TestContext.Out.WriteLine("Consume message rate: {0:F2} (msg/s)", messageCount * 1000 / totalTime.TotalMilliseconds);


            await harness.Stop();
        }

        [Test]
        [Explicit]
        public async Task Should_support_delayed_message_scheduler()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
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

                    x.AddConsumer<PingConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            await harness.Start();

            IConsumerTestHarness<PingConsumer> consumerHarness = harness.GetConsumerHarness<PingConsumer>();

            {
                await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

                await scheduler.SchedulePublish(TimeSpan.FromSeconds(8), new PingMessage());

                await dbContext.SaveChangesAsync(harness.CancellationToken);
            }

            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                Assert.That(await consumerHarness.Consumed.Any<PingMessage>(cts.Token), Is.False);
            }

            Assert.That(await consumerHarness.Consumed.Any<PingMessage>(), Is.True);
        }

        [Explicit]
        [Test]
        public async Task Should_work_without_starting_the_bus()
        {
            await using var provider = new ServiceCollection()
                .AddBusOutboxServices()
                .AddTelemetryListener()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.DisableInboxCleanupService();

                        o.UseBusOutbox(bo => bo.DisableDeliveryService());
                    });

                    x.AddConsumer<PingConsumer>();
                })
                .RemoveMassTransitHostedService()
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            {
                await using var dbContext = harness.Scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var publishEndpoint = harness.Scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(new PingMessage());

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

                await dbContext.SaveChangesAsync(harness.CancellationToken);
            }

            {
                using var scope = provider.CreateScope();

                await using var dbContext = scope.ServiceProvider.GetRequiredService<ReliableDbContext>();

                var count = await dbContext.Set<OutboxMessage>().CountAsync();

                Assert.That(count, Is.EqualTo(1));
            }

            await harness.Stop();
        }


        class PingBaggageConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<string> _baggage;

            public PingBaggageConsumer(TaskCompletionSource<string> baggage)
            {
                _baggage = baggage;
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                KeyValuePair<string, string>? pair = Activity.Current?.Baggage.FirstOrDefault(x => x.Key.Equals("Suitcase"));
                if (pair != null)
                    _baggage.TrySetResult(pair.Value.Value);

                return Task.CompletedTask;
            }
        }


        class PingConsumer :
            IConsumer<PingMessage>
        {
            public Task Consume(ConsumeContext<PingMessage> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    public static class BusOutboxTestExtensions
    {
        public static IServiceCollection AddBusOutboxServices(this IServiceCollection services)
        {
            services.AddDbContext<ReliableDbContext>(builder =>
            {
                ReliableDbContextFactory.Apply(builder);
                //
                // builder.UseNpgsql("host=localhost;user id=postgres;password=Password12!;database=MassTransitUnitTests;", options =>
                // {
                //     options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                //     options.MigrationsHistoryTable($"__{nameof(ReliableDbContext)}");
                //
                //     options.EnableRetryOnFailure(5);
                //     options.MinBatchSize(1);
                // });
            });
            services.AddHostedService<MigrationHostedService<ReliableDbContext>>();

            services.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

            return services;
        }
    }
}
