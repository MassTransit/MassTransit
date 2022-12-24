namespace MassTransitBenchmark.Latency;

using System;
using System.Threading.Tasks;
using BusOutbox;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;


public class KafkaMessageLatencyTransport :
    IMessageLatencyTransport
{
    readonly KafkaOptionSet _options;
    readonly IMessageLatencySettings _settings;
    ITopicProducer<LatencyTestMessage> _producer;
    ServiceProvider _provider;
    AsyncServiceScope _scope;

    public KafkaMessageLatencyTransport(KafkaOptionSet options, IMessageLatencySettings settings)
    {
        _options = options;
        _settings = settings;
    }

    public Task Send(LatencyTestMessage message)
    {
        return _producer.Produce(message);
    }

    public async Task Start(Action<IReceiveEndpointConfigurator> callback, IReportConsumerMetric reportConsumerMetric)
    {
        _provider = new ServiceCollection()
            .AddTextLogger(Console.Out)
            .AddSingleton(reportConsumerMetric)
            .AddMassTransit(x =>
            {
                x.UsingInMemory();

                x.AddRider(r =>
                {
                    r.AddConsumer<MessageLatencyConsumer>();

                    r.AddProducer<LatencyTestMessage>(_options.Topic);

                    r.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<LatencyTestMessage>(_options.Topic, nameof(KafkaMessageLatencyTransport), e =>
                        {
                            e.PrefetchCount = _settings.PrefetchCount;
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            e.CreateIfMissing(p =>
                            {
                                p.NumPartitions = _options.PartitionCount;
                            });

                            if (_settings.ConcurrencyLimit > 0)
                                e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                            e.ConfigureConsumer<MessageLatencyConsumer>(context);
                        });
                    });
                });
            })
            .BuildServiceProvider(true);

        await _provider.StartHostedServices();

        _scope = _provider.CreateAsyncScope();
        _producer = _scope.ServiceProvider.GetRequiredService<ITopicProducer<LatencyTestMessage>>();
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();

        await _provider.StopHostedServices();
    }
}
