namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    class ServiceBusMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly ServiceBusHostSettings _hostSettings;
        readonly IMessageLatencySettings _settings;
        readonly bool _split;
        IBusControl _busControl;
        IBusControl _outboundBus;
        Uri _targetAddress;
        Task<ISendEndpoint> _targetEndpoint;

        public ServiceBusMessageLatencyTransport(ServiceBusOptionSet hostSettings, IMessageLatencySettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;

            _split = hostSettings.Split;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                x.Host(_hostSettings);

                x.ReceiveEndpoint("latency_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PrefetchCount = _settings.PrefetchCount;

                    if (_settings.ConcurrencyLimit > 0)
                        e.ConcurrentMessageLimit = _settings.ConcurrencyLimit;

                    callback(e);

                    _targetAddress = e.InputAddress;
                });

                x.PrefetchCount = _settings.PrefetchCount;
            });

            await _busControl.StartAsync();

            if (_split)
            {
                _outboundBus = Bus.Factory.CreateUsingAzureServiceBus(x =>
                {
                    x.Host(_hostSettings);
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
