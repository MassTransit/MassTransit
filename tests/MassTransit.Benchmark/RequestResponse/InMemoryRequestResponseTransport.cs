namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public class InMemoryRequestResponseTransport :
        IRequestResponseTransport
    {
        readonly InMemoryOptionSet _optionSet;
        IBusControl _busControl;

        Task<IClientFactory> _clientFactory;
        IRequestResponseSettings _settings;

        Uri _targetEndpointAddress;

        public InMemoryRequestResponseTransport(InMemoryOptionSet optionSet, IRequestResponseSettings settings)
        {
            _optionSet = optionSet;
            _settings = settings;
        }

        public void GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.AutoStart = true;
                x.ConcurrentMessageLimit = _optionSet.TransportConcurrencyLimit;

                x.ReceiveEndpoint("rpc_consumer", e =>
                {
                    callback(e);
                    _targetEndpointAddress = e.InputAddress;
                });
            });

            _busControl.Start();

            _clientFactory = _busControl.CreateReplyToClientFactory();
        }

        public async Task<IRequestClient<T>> GetRequestClient<T>(TimeSpan settingsRequestTimeout)
            where T : class
        {
            var clientFactory = await _clientFactory;

            return clientFactory.CreateRequestClient<T>(_targetEndpointAddress, settingsRequestTimeout);
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}
