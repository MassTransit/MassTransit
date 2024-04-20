namespace MassTransit.DbTransport.Tests;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Internals;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_partition_keys<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_consume_a_lot_of_published_messages()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddTaskCompletionSource<ConsumeContext<PartitionedTestMessage>>();

                x.AddConsumer<PartitionedConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ReceiveEndpoint("partitioned-input-queue", e =>
                    {
                        e.PrefetchCount = 10;
                        e.ConcurrentMessageLimit = 10;
                        e.PurgeOnStartup = true;

                        e.SetReceiveMode(SqlReceiveMode.Partitioned);

                        e.ConfigureConsumer<PartitionedConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:partitioned-input-queue"));

        for (var i = 0; i < MessageLimit; i++)
            await endpoint.Send(new PartitionedTestMessage(i + 1), x => x.SetPartitionKey((i % NumKeys).ToString()), harness.CancellationToken);

        await provider.GetTask<ConsumeContext<PartitionedTestMessage>>();

        IList<IReceivedMessage<PartitionedTestMessage>> receivedMessages = await harness.Consumed.SelectAsync<PartitionedTestMessage>().ToListAsync();
        Assert.That(receivedMessages, Has.Count.EqualTo(MessageLimit));
        var result = new int[NumKeys];

        foreach (IReceivedMessage<PartitionedTestMessage> receivedMessage in receivedMessages)
        {
            ConsumeContext<PartitionedTestMessage> context = receivedMessage.Context;
            var key = int.TryParse(context.PartitionKey(), out var value) ? value : 0;
            Assert.That(context.Message.Index, Is.GreaterThan(result[key]));
            result[key] = context.Message.Index;
        }
    }

    const int MessageLimit = 30;
    const int NumKeys = 2;

    readonly T _configuration;

    public Using_partition_keys()
    {
        _configuration = new T();
    }


    public record PartitionedTestMessage(int Index);


    class PartitionedConsumer :
        IConsumer<PartitionedTestMessage>
    {
        static int _index = MessageLimit;
        readonly TaskCompletionSource<ConsumeContext<PartitionedTestMessage>> _taskCompletionSource;

        public PartitionedConsumer(TaskCompletionSource<ConsumeContext<PartitionedTestMessage>> taskCompletionSource)
        {
            _taskCompletionSource = taskCompletionSource;
        }

        public async Task Consume(ConsumeContext<PartitionedTestMessage> context)
        {
            if (Interlocked.Decrement(ref _index) <= 0)
                _taskCompletionSource.TrySetResult(context);

            await Task.Delay(4);
        }
    }
}
