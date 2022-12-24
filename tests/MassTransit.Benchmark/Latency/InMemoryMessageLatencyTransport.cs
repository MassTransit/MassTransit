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
        ISendEndpoint _targetEndpoint;

        public InMemoryMessageLatencyTransport(InMemoryOptionSet optionSet, IMessageLatencySettings settings)
        {
            _optionSet = optionSet;
            _settings = settings;
        }

        public Task Send(LatencyTestMessage message)
        {
            return _targetEndpoint.Send(message);
        }

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();
        }

        public async Task Start(Action<IReceiveEndpointConfigurator> callback, IReportConsumerMetric reportConsumerMetric)
        {
            _busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ConcurrentMessageLimit = _optionSet.TransportConcurrencyLimit;

                x.ReceiveEndpoint("latency_consumer", e =>
                {
                    callback(e);
                    _targetAddress = e.InputAddress;
                });
            });

            await _busControl.StartAsync();

            _targetEndpoint = await _busControl.GetSendEndpoint(_targetAddress);
        }
    }
}
