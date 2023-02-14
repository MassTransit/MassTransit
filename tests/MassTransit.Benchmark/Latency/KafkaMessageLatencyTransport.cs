namespace MassTransitBenchmark.Latency;

using System;
using System.Threading.Tasks;
using BusOutbox;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using MassTransit;
using MassTransit.KafkaIntegration;
using MassTransit.Middleware;
using Microsoft.Extensions.DependencyInjection;


public class KafkaMessageLatencyTransport :
    IMessageLatencyTransport
{
    readonly IAdminClient _adminClient;
    readonly ClientConfig _clientConfig;
    readonly Murmur3UnsafeHashGenerator _hashGenerator;
    readonly KafkaKeyResolver<int, LatencyTestMessage> _keyResolver;
    readonly KafkaOptionSet _options;
    readonly IPipe<KafkaSendContext<LatencyTestMessage>> _partitionPipe;
    readonly IMessageLatencySettings _settings;
    ITopicProducer<LatencyTestMessage> _producer;
    ServiceProvider _provider;
    AsyncServiceScope _scope;

    public KafkaMessageLatencyTransport(KafkaOptionSet options, IMessageLatencySettings settings)
    {
        _options = options;
        _settings = settings;
        _partitionPipe = GetPartitionPipe();
        _keyResolver = GetKeyResolver();
        _hashGenerator = new Murmur3UnsafeHashGenerator();
        _clientConfig = new ClientConfig { BootstrapServers = _options.Host };

        _adminClient = new AdminClientBuilder(new AdminClientConfig(_clientConfig))
            .Build();
    }

    public Task Send(LatencyTestMessage message)
    {
        return _producer.Produce(message, _partitionPipe);
    }

    public async Task Start(Action<IReceiveEndpointConfigurator> callback, IReportConsumerMetric reportConsumerMetric)
    {
        await CreateTopic();

        _provider = new ServiceCollection()
            .AddTextLogger(Console.Out)
            .AddSingleton(reportConsumerMetric)
            .AddMassTransit(x =>
            {
                x.UsingInMemory();

                x.AddRider(r =>
                {
                    r.AddConsumer<MessageLatencyConsumer>();

                    r.AddProducer(_options.Topic, _keyResolver);

                    r.UsingKafka(_clientConfig, (context, k) =>
                    {
                        k.TopicEndpoint<LatencyTestMessage>(_options.Topic, nameof(KafkaMessageLatencyTransport), e =>
                        {
                            e.PrefetchCount = _settings.PrefetchCount;
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;

                            if (_options.ConcurrentConsumerLimit > 0)
                                e.ConcurrentConsumerLimit = _options.ConcurrentConsumerLimit;

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

        await DeleteTopic();

        _adminClient.Dispose();
    }

    async Task DeleteTopic()
    {
        try
        {
            await _adminClient.DeleteTopicsAsync(new[] { _options.Topic });
        }
        catch (DeleteTopicsException)
        {
        }
    }

    async Task CreateTopic()
    {
        try
        {
            await _adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = _options.Topic,
                    NumPartitions = _options.PartitionCount,
                    ReplicationFactor = 1
                }
            });
        }
        catch (CreateTopicsException)
        {
        }

        // Adding some delays for kafka to settle
        await Task.Delay(TimeSpan.FromMilliseconds(100));
    }

    KafkaKeyResolver<int, LatencyTestMessage> GetKeyResolver()
    {
        if (_options.KeysCount > 1)
        {
            return context =>
            {
                var partitionKey = context.Message.CorrelationId.ToByteArray();
                var hash = partitionKey?.Length > 0 ? _hashGenerator.Hash(partitionKey) : 0;
                return (int)(hash % _options.KeysCount);
            };
        }

        return _ => 1;
    }

    IPipe<KafkaSendContext<LatencyTestMessage>> GetPartitionPipe()
    {
        if (_options.PartitionCount > 1)
        {
            return Pipe.Execute<KafkaSendContext<LatencyTestMessage>>(ctx =>
            {
                var partitionKey = ctx.Message.CorrelationId.ToByteArray();
                var hash = partitionKey?.Length > 0 ? _hashGenerator.Hash(partitionKey) : 0;
                ctx.Partition = (int)(hash % _options.PartitionCount);
            });
        }

        return Pipe.Empty<KafkaSendContext<LatencyTestMessage>>();
    }
}
