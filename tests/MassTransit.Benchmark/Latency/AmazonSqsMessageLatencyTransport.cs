namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using BusOutbox;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;


    public class AmazonSqsMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly AmazonSqsHostSettings _hostSettings;
        readonly IMessageLatencySettings _settings;
        Uri _targetAddress;
        ISendEndpoint _targetEndpoint;
        ServiceProvider _provider;
        AsyncServiceScope _scope;

        public AmazonSqsMessageLatencyTransport(AmazonSqsHostSettings hostSettings, IMessageLatencySettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task Send(LatencyTestMessage message)
        {
            return _targetEndpoint.Send(message);
        }

        public async Task Start(Action<IReceiveEndpointConfigurator> callback, IReportConsumerMetric reportConsumerMetric)
        {
            _provider = new ServiceCollection()
                .AddTextLogger(Console.Out)
                .AddSingleton(reportConsumerMetric)
                .AddMassTransit(x =>
                {
                    x.AddConsumer<MessageLatencyConsumer>();

                    x.UsingAmazonSqs((context, cfg) =>
                    {
                        cfg.Host(_hostSettings);

                        cfg.ReceiveEndpoint("latency_consumer" + (_settings.Durable ? "" : "_express"), e =>
                        {
                            e.Durable = _settings.Durable;
                            e.PrefetchCount = _settings.PrefetchCount;

                            if (_settings.ConcurrencyLimit > 0)
                                e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                            callback(e);

                            _targetAddress = e.InputAddress;
                        });
                    });
                })
                .BuildServiceProvider(true);

            await _provider.StartHostedServices();

            _scope = _provider.CreateAsyncScope();

            _targetEndpoint = await _scope.ServiceProvider.GetRequiredService<ISendEndpointProvider>().GetSendEndpoint(_targetAddress);
        }

        public async ValueTask DisposeAsync()
        {
            await _scope.DisposeAsync();

            await _provider.StopHostedServices();
        }
    }
}
