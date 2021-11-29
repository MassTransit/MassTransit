namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    class InMemoryMessageLatencyTransport : IMessageLatencyTransport
    {
        readonly InMemoryOptionSet _optionSet;
        readonly IMessageLatencySettings _settings;
        IBusControl _busControl;
        Uri _targetAddress;
        Task<ISendEndpoint> _targetEndpoint;

        public InMemoryMessageLatencyTransport(InMemoryOptionSet optionSet, IMessageLatencySettings settings)
        {
            _optionSet = optionSet;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();
        }

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.TransportConcurrencyLimit = _optionSet.TransportConcurrencyLimit;

                x.ReceiveEndpoint("latency_consumer", e =>
                {
                    callback(e);
                    _targetAddress = e.InputAddress;
                });
            });

            await _busControl.StartAsync();

            _targetEndpoint = _busControl.GetSendEndpoint(_targetAddress);
        }
    }
}