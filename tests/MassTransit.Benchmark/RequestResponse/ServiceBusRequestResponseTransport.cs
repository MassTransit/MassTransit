namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public class ServiceBusRequestResponseTransport :
        IRequestResponseTransport
    {
        readonly ServiceBusHostSettings _hostSettings;
        readonly IRequestResponseSettings _settings;
        IBusControl _busControl;
        IClientFactory _clientFactory;

        Uri _targetEndpointAddress;

        public ServiceBusRequestResponseTransport(ServiceBusHostSettings hostSettings, IRequestResponseSettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<IRequestClient<T>> GetRequestClient<T>(TimeSpan settingsRequestTimeout)
            where T : class
        {
            return Task.FromResult(_clientFactory.CreateRequestClient<T>(_targetEndpointAddress, settingsRequestTimeout));
        }

        public void GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                x.AutoStart = true;
                x.Host(_hostSettings);

                x.ReceiveEndpoint("rpc_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PrefetchCount = _settings.PrefetchCount;
                    if (_settings.ConcurrencyLimit > 0)
                        e.MaxConcurrentCalls = _settings.ConcurrencyLimit;

                    callback(e);

                    _targetEndpointAddress = e.InputAddress;
                });
            });

            _busControl.Start();

            _clientFactory = _busControl.CreateClientFactory();
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}
