namespace MassTransit.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Internals;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework;
using TestFramework.Messages;


[TestFixture]
[Category("Flaky")]
public class Using_delayed_redelivery :
    InMemoryTestFixture
{
    [Test]
    public async Task Should_use_the_correct_intervals_for_each_redelivery()
    {
        await InputQueueSendEndpoint.Send(new PingMessage());

        await Task.WhenAll(_received.Select(x => x.Task));

        Assert.Multiple(() =>
        {
            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1.9)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));
        });

        TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
        TestContext.Out.WriteLine("Interval: {0}", _timestamps[2] - _timestamps[1]);
        TestContext.Out.WriteLine("Interval: {0}", _timestamps[3] - _timestamps[2]);
    }

    TaskCompletionSource<ConsumeContext<PingMessage>>[] _received;
    int _count;
    DateTime[] _timestamps;

    protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
    {
        configurator.UseNewtonsoftJsonSerializer();

        _count = 0;
        _received = new[]
        {
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>()
        };
        _timestamps = new DateTime[4];

        configurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 3000));

        configurator.Handler<PingMessage>(async context =>
        {
            _received[_count].TrySetResult(context);
            _timestamps[_count] = DateTime.Now;

            _count++;

            throw new IntentionalTestException("I'm so not ready for this jelly.");
        });
    }
}


[TestFixture]
[Category("Flaky")]
public class Using_delayed_redelivery_with_system_text_json :
    InMemoryTestFixture
{
    [Test]
    public async Task Should_use_the_correct_intervals_for_each_redelivery()
    {
        await InputQueueSendEndpoint.Send(new PingMessage());

        await Task.WhenAll(_received.Select(x => x.Task));

        Assert.Multiple(() =>
        {
            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1.9)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));
        });

        TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
        TestContext.Out.WriteLine("Interval: {0}", _timestamps[2] - _timestamps[1]);
        TestContext.Out.WriteLine("Interval: {0}", _timestamps[3] - _timestamps[2]);
    }

    TaskCompletionSource<ConsumeContext<PingMessage>>[] _received;
    int _count;
    DateTime[] _timestamps;

    protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
    {
        _count = 0;
        _received = new[]
        {
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>()
        };
        _timestamps = new DateTime[4];

        configurator.UseDelayedRedelivery(r => r.Intervals(1000, 2000, 3000));

        configurator.Handler<PingMessage>(async context =>
        {
            _received[_count].TrySetResult(context);
            _timestamps[_count] = DateTime.Now;

            _count++;

            throw new IntentionalTestException("I'm so not ready for this jelly.");
        });
    }
}


[TestFixture]
[Category("Flaky")]
public class Using_delayed_redelivery_with_new_message_id :
    InMemoryTestFixture
{
    [Test]
    public async Task Should_use_the_correct_intervals_for_each_redelivery()
    {
        Guid messageId = NewId.NextGuid();

        await InputQueueSendEndpoint.Send(new PingMessage(), x => x.MessageId = messageId);

        await Task.WhenAll(_received.Select(x => x.Task));

        Assert.Multiple(() =>
        {
            Assert.That(_timestamps[1] - _timestamps[0], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.9)));
            Assert.That(_timestamps[2] - _timestamps[1], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1.9)));
            Assert.That(_timestamps[3] - _timestamps[2], Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2.9)));
        });

        TestContext.Out.WriteLine("Interval: {0}", _timestamps[1] - _timestamps[0]);
        TestContext.Out.WriteLine("Interval: {0}", _timestamps[2] - _timestamps[1]);
        TestContext.Out.WriteLine("Interval: {0}", _timestamps[3] - _timestamps[2]);

        Assert.Multiple(() =>
        {
            Assert.That(_received[0].Task.Result.MessageId.Value, Is.EqualTo(messageId));
            Assert.That(_received[1].Task.Result.MessageId.Value, Is.Not.EqualTo(messageId));
            Assert.That(_received[2].Task.Result.MessageId.Value, Is.Not.EqualTo(messageId));
            Assert.That(_received[3].Task.Result.MessageId.Value, Is.Not.EqualTo(messageId));

            Assert.That(_received[1].Task.Result.GetHeader(MessageHeaders.OriginalMessageId, default(Guid?)), Is.EqualTo((Guid?)messageId));
            Assert.That(_received[2].Task.Result.GetHeader(MessageHeaders.OriginalMessageId, default(Guid?)), Is.EqualTo((Guid?)messageId));
            Assert.That(_received[3].Task.Result.GetHeader(MessageHeaders.OriginalMessageId, default(Guid?)), Is.EqualTo((Guid?)messageId));
        });
    }

    TaskCompletionSource<ConsumeContext<PingMessage>>[] _received;
    int _count;
    DateTime[] _timestamps;

    protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
    {
        _count = 0;
        _received = new[]
        {
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>(),
            GetTask<ConsumeContext<PingMessage>>()
        };
        _timestamps = new DateTime[4];

        configurator.UseDelayedRedelivery(r =>
        {
            r.ReplaceMessageId = true;
            r.Intervals(1000, 2000, 3000);
        });

        configurator.Handler<PingMessage>(async context =>
        {
            _received[_count].TrySetResult(context);
            _timestamps[_count] = DateTime.Now;

            _count++;

            throw new IntentionalTestException("I'm so not ready for this jelly.");
        });
    }
}


[TestFixture]
public class Using_multiple_redelivery_filters
{
    [Test]
    public async Task Should_play_nicely_together()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConfigureEndpointsCallback((context, _, cfg) =>
                {
                    cfg.UseDelayedRedelivery(r =>
                    {
                        r.Handle<ExceptionA>();
                        r.Interval(4, 20);
                    });
                    cfg.UseDelayedRedelivery(r =>
                    {
                        r.Handle<ExceptionB>();
                        r.Interval(2, 20);
                    });
                });

                x.AddHandler(async (ConsumeContext<MessageA> message) => throw new ExceptionA());
                x.AddHandler(async (ConsumeContext<MessageB> message) => throw new ExceptionB());
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        // await harness.Bus.Publish(new MessageA());
        //
        // Assert.That(await harness.Published.Any<Fault<MessageA>>());
        //
        // IPublishedMessage<Fault<MessageA>> contextA = await harness.Published.SelectAsync<Fault<MessageA>>().First();
        //
        // int? redeliveryCount = contextA.Context.Headers.Get<int>(MessageHeaders.FaultRedeliveryCount);
        //
        // Assert.That(redeliveryCount, Is.EqualTo(4));
        //
        await harness.Bus.Publish(new MessageB());


        Assert.That(await harness.Published.Any<Fault<MessageB>>());

        IList<IPublishedMessage<Fault<MessageB>>> faults = await harness.Published.SelectAsync<Fault<MessageB>>().Take(3).ToListAsync();

        Assert.That(faults.Count, Is.EqualTo(1));

        var redeliveryCount = faults[0].Context.Headers.Get<int>(MessageHeaders.FaultRedeliveryCount);

        Assert.That(redeliveryCount, Is.EqualTo(2));
    }


    public record MessageA;


    public record MessageB;


    public class ExceptionA : Exception
    {
    }


    public class ExceptionB : Exception
    {
    }
}
