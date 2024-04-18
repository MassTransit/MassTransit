#nullable enable
namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using LazySubjects;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    public class LazyMessageType_Specs
    {
        [Test]
        public async Task Should_not_recursively_call_into_the_topology_types()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddServiceBusMessageScheduler();
                    x.UsingTestAzureServiceBus((context, cfg) =>
                    {
                        cfg.UseServiceBusMessageScheduler();

                        cfg.UseMessageRetry(retryConfiguration =>
                        {
                            retryConfiguration.Exponential(5, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(500));
                        });

                        cfg.Publish(typeof(IntegrationEvent), m => m.Exclude = true);
                        cfg.Publish(typeof(IIntegrationEvent), m => m.Exclude = true);

                        cfg.SetNamespaceSeparatorTo("-");
                        cfg.MessageTopology.SetEntityNameFormatter(new CustomEntityNameFormatter(cfg.MessageTopology.EntityNameFormatter));
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

            object sampleEvent = new SampleIntegrationEvent(NewId.NextGuid(), DateTimeOffset.UtcNow, "sample");
            object sampleEvent2 = new SampleIntegrationEvent2(NewId.NextGuid(), DateTimeOffset.UtcNow, "sample");

            var task1 = scheduler.SchedulePublish(DateTime.UtcNow.AddSeconds(5), sampleEvent, harness.CancellationToken);
            var task2 = scheduler.SchedulePublish(DateTime.UtcNow.AddSeconds(5), sampleEvent2, harness.CancellationToken);
            var task3 = scheduler.SchedulePublish(DateTime.UtcNow.AddSeconds(5), sampleEvent, harness.CancellationToken);
            var task4 = scheduler.SchedulePublish(DateTime.UtcNow.AddSeconds(5), sampleEvent2, harness.CancellationToken);

            await Task.WhenAll(task1, task2, task3, task4);
        }
    }


    namespace LazySubjects
    {
        using System;
        using System.Reflection;


        public interface IIntegrationEvent
        {
            public Guid EventId { get; init; }
            public DateTimeOffset EventDate { get; init; }
        }


        [Serializable]
        public abstract record IntegrationEvent(
            Guid EventId,
            DateTimeOffset EventDate);


        public sealed record SampleIntegrationEvent(
            Guid EventId,
            DateTimeOffset EventDate,
            string ResourceId) : IntegrationEvent(EventId, EventDate),
            IIntegrationEvent
        {
            public static string EventName()
            {
                return "my.sample-event";
            }
        }

        public sealed record SampleIntegrationEvent2(
            Guid EventId,
            DateTimeOffset EventDate,
            string ResourceId) : IntegrationEvent(EventId, EventDate),
            IIntegrationEvent
        {
            public static string EventName()
            {
                return "my.sample-event.2";
            }
        }


        public sealed class CustomEntityNameFormatter : IEntityNameFormatter
        {
            readonly IEntityNameFormatter _original;

            public CustomEntityNameFormatter(IEntityNameFormatter original)
            {
                _original = original;
            }

            public string FormatEntityName<T>()
            {
                string? result = null;
                if (typeof(T).GetInterface(nameof(IIntegrationEvent)) != null)
                    result = typeof(T).GetMethod("EventName", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null) as string;

                return result ?? _original.FormatEntityName<T>();
            }
        }
    }
}
