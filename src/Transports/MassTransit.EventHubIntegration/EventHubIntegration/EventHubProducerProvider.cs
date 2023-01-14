namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Observables;
    using Transports;


    public class EventHubProducerProvider :
        IEventHubProducerProvider
    {
        readonly IBusInstance _busInstance;
        readonly IEventHubProducerCache<Uri> _cache;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservable;

        public EventHubProducerProvider(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _cache = new EventHubProducerCache<Uri>();
            _sendObservable = new SendObservable();
        }

        public Task<IEventHubProducer> GetProducer(Uri address)
        {
            return _cache.GetProducer(address, CreateProducer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        Task<IEventHubProducer> CreateProducer(Uri address)
        {
            var topicAddress = NormalizeAddress(_busInstance.HostConfiguration.HostAddress, address);
            var transportContext = _hostConfiguration.CreateSendTransportContext(topicAddress.EventHubName, _busInstance);
            IEventHubProducer producer = new EventHubProducer(transportContext, transportContext.ConnectSendObserver(_sendObservable));
            return Task.FromResult(producer);
        }

        static EventHubEndpointAddress NormalizeAddress(Uri hostAddress, Uri address)
        {
            return new EventHubEndpointAddress(hostAddress, address);
        }
    }
}
