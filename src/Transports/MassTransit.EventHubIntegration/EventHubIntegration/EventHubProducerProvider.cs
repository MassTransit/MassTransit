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
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservable;
        readonly ISendPipe _sendPipe;
        readonly ISerialization _serializers;

        public EventHubProducerProvider(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance, ISendPipe sendPipe,
            SendObservable sendObservable, ISerialization serializers)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _sendPipe = sendPipe;
            _sendObservable = sendObservable;
            _serializers = serializers;
        }

        public Task<IEventHubProducer> GetProducer(Uri address)
        {
            var context = new EventHubProducerSendTransportContext(_hostConfiguration, _sendPipe, _busInstance.HostConfiguration, address, _serializers);

            if (_sendObservable.Count > 0)
                context.ConnectSendObserver(_sendObservable);

            IEventHubProducer eventHubProducer = new EventHubProducer(context);
            return Task.FromResult(eventHubProducer);
        }
    }
}
