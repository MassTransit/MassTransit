namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.AmazonSqsTransport;


    public class AmazonSqsMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly AmazonSqsHostSettings _hostSettings;
        readonly IMessageLatencySettings _settings;
        IBusControl _busControl;
        Uri _targetAddress;
        Task<ISendEndpoint> _targetEndpoint;

        public AmazonSqsMessageLatencyTransport(AmazonSqsHostSettings hostSettings, IMessageLatencySettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingAmazonSqs(x =>
            {
                x.Host(_hostSettings);

                x.ReceiveEndpoint("latency_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.Durable = _settings.Durable;
                    e.PrefetchCount = _settings.PrefetchCount;

                    if (_settings.ConcurrencyLimit > 0)
                        e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                    callback(e);

                    _targetAddress = e.InputAddress;
                });
            });

            await _busControl.StartAsync();

            _targetEndpoint = _busControl.GetSendEndpoint(_targetAddress);
        }

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();
        }
    }
}