namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;


    class RabbitMqMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly RabbitMqHostSettings _hostSettings;
        readonly IMessageLatencySettings _settings;
        readonly bool _split;
        IBusControl _busControl;
        IBusControl _outboundBus;
        Uri _targetAddress;
        Task<ISendEndpoint> _targetEndpoint;

        public RabbitMqMessageLatencyTransport(RabbitMqOptionSet hostSettings, IMessageLatencySettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;

            _split = hostSettings.Split;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(_hostSettings);

                x.ReceiveEndpoint("latency_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PurgeOnStartup = true;
                    e.Durable = _settings.Durable;
                    e.PrefetchCount = _settings.PrefetchCount;

                    if (_settings.ConcurrencyLimit > 0)
                        e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                    callback(e);

                    _targetAddress = e.InputAddress;
                });
            });

            await _busControl.StartAsync();

            if (_split)
            {
                _outboundBus = Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    x.Host(_hostSettings);

                    x.PrefetchCount = _settings.PrefetchCount;
                });

                await _outboundBus.StartAsync();

                _targetEndpoint = _outboundBus.GetSendEndpoint(_targetAddress);
            }
            else
                _targetEndpoint = _busControl.GetSendEndpoint(_targetAddress);
        }

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();

            if (_outboundBus != null)
                await _outboundBus.StopAsync();
        }
    }
}