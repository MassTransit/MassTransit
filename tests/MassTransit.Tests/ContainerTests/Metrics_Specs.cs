namespace MassTransit.Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Microsoft.Extensions.Options;
using Monitoring;
using NUnit.Framework;
using OpenTelemetry.Metrics;
using TestFramework;
using TestFramework.Messages;


[TestFixture]
[Explicit]
public class ConsumeMetrics_Specs
{
    [Test]
    public async Task Should_be_able_to_add_custom_tag_to_consume_metrics()
    {
        await using var provider = CreateServiceCollection()
            .AddMassTransitTestHarness(configurator =>
            {
                configurator.AddHandler(async (PingMessage _) =>
                {
                });

                configurator.AddConfigureEndpointsCallback((_, _, e) => e.UseFilter(new MetricsFilter()));
            })
            .BuildServiceProvider();

        var testHarness = provider.GetTestHarness();

        await testHarness.Start();

        var instrumentationOptions = provider.GetRequiredService<IOptions<InstrumentationOptions>>();

        using MetricCollector<long> collector = GetMetricCollector<long>(provider, instrumentationOptions.Value.ConsumeTotal);

        await testHarness.Bus.Publish(new PingMessage());

        Assert.That(await testHarness.Consumed.Any<PingMessage>(), Is.True);

        IReadOnlyList<CollectedMeasurement<long>> metrics = collector.GetMeasurementSnapshot();

        Assert.That(metrics, Has.Count.EqualTo(1));

        foreach (CollectedMeasurement<long> metric in metrics)
            Assert.That(metric.Tags, Contains.Key(TagName));
    }

    [Test]
    public async Task Should_be_able_to_produce_consume_exception_metrics()
    {
        await using var provider = CreateServiceCollection()
            .AddMassTransitTestHarness(configurator => configurator.AddHandler(async (PingMessage _) => throw new IntentionalTestException()))
            .BuildServiceProvider();

        var testHarness = provider.GetTestHarness();

        await testHarness.Start();

        var instrumentationOptions = provider.GetRequiredService<IOptions<InstrumentationOptions>>();

        using MetricCollector<long> totalCollector = GetMetricCollector<long>(provider, instrumentationOptions.Value.ConsumeTotal);
        using MetricCollector<long> faultCollector = GetMetricCollector<long>(provider, instrumentationOptions.Value.ConsumeFaultTotal);

        await testHarness.Bus.Publish(new PingMessage());

        Assert.That(await testHarness.Consumed.Any<PingMessage>(), Is.True);

        IReadOnlyList<CollectedMeasurement<long>> metrics = totalCollector.GetMeasurementSnapshot();
        IReadOnlyList<CollectedMeasurement<long>> faults = faultCollector.GetMeasurementSnapshot();

        Assert.Multiple(() =>
        {
            Assert.That(metrics, Has.Count.EqualTo(1));
            Assert.That(faults, Has.Count.EqualTo(1));
        });

        foreach (CollectedMeasurement<long> metric in metrics)
            Assert.That(metric.Value, Is.EqualTo(1));

        foreach (CollectedMeasurement<long> metric in faults)
        {
            Assert.Multiple(() =>
            {
                Assert.That(metric.Value, Is.EqualTo(1));
                Assert.That(metric.Tags, Does.ContainKey(instrumentationOptions.Value.ExceptionTypeLabel).WithValue(nameof(IntentionalTestException)));
            });
        }
    }

    [Test]
    public async Task Should_be_able_to_produce_consume_metrics()
    {
        await using var provider = CreateServiceCollection()
            .AddSingleton(provider => provider.GetTestHarness().GetTask<ConsumeContext<PingMessage>>())
            .AddSingleton(provider => provider.GetTestHarness().GetTask<bool>())
            .AddMassTransitTestHarness(configurator => configurator.AddConsumer<PingConsumer>())
            .BuildServiceProvider();

        var testHarness = provider.GetTestHarness();
        await testHarness.Start();

        var instrumentationOptions = provider.GetRequiredService<IOptions<InstrumentationOptions>>();

        using MetricCollector<long> totalCollector = GetMetricCollector<long>(provider, instrumentationOptions.Value.ConsumeTotal);
        using MetricCollector<double> durationCollector = GetMetricCollector<double>(provider, instrumentationOptions.Value.ConsumeDuration);
        using MetricCollector<long> inProgressCollector = GetMetricCollector<long>(provider, instrumentationOptions.Value.ConsumerInProgress);

        await testHarness.Bus.Publish(new PingMessage());

        await provider.GetTask<ConsumeContext<PingMessage>>();

        IReadOnlyList<CollectedMeasurement<long>> inProgress = inProgressCollector.GetMeasurementSnapshot();

        foreach (CollectedMeasurement<long> metric in inProgress)
            Assert.That(metric.Value, Is.EqualTo(1));

        var completed = provider.GetRequiredService<TaskCompletionSource<bool>>();
        completed.TrySetResult(true);

        Assert.That(await testHarness.Consumed.Any<PingMessage>(), Is.True);

        IReadOnlyList<CollectedMeasurement<long>> metrics = totalCollector.GetMeasurementSnapshot();
        inProgress = inProgressCollector.GetMeasurementSnapshot();

        IReadOnlyList<CollectedMeasurement<double>> duration = durationCollector.GetMeasurementSnapshot();

        Assert.Multiple(() =>
        {
            Assert.That(metrics, Has.Count.EqualTo(1));
            Assert.That(duration, Has.Count.EqualTo(1));
            Assert.That(inProgress, Has.Count.EqualTo(2));
        });

        foreach (CollectedMeasurement<long> metric in metrics)
            Assert.That(metric.Value, Is.EqualTo(1));

        foreach (CollectedMeasurement<double> metric in duration)
            Assert.That(metric.Value, Is.GreaterThan(0));
    }

    const string TagName = "custom-metric-name";


    class PingConsumer :
        IConsumer<PingMessage>
    {
        readonly TaskCompletionSource<bool> _completed;
        readonly TaskCompletionSource<ConsumeContext<PingMessage>> _ready;

        public PingConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> ready, TaskCompletionSource<bool> completed)
        {
            _ready = ready;
            _completed = completed;
        }

        public Task Consume(ConsumeContext<PingMessage> context)
        {
            _ready.TrySetResult(context);
            return _completed.Task;
        }
    }


    class MetricsFilter :
        IFilter<ConsumeContext>
    {
        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            context.AddMetricTags(TagName, "test");
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }


    protected static ServiceCollection CreateServiceCollection()
    {
        var collection = new ServiceCollection();
        collection.AddMetrics();
        collection.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddMeter(InstrumentationOptions.MeterName)
                .AddConsoleExporter());
        return collection;
    }

    protected static MetricCollector<T> GetMetricCollector<T>(IServiceProvider provider, string instrumentation)
        where T : struct
    {
        var meterFactory = provider.GetRequiredService<IMeterFactory>();
        return new MetricCollector<T>(meterFactory, InstrumentationOptions.MeterName, instrumentation);
    }
}
