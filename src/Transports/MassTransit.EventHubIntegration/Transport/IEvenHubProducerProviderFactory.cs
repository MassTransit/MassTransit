namespace MassTransit.EventHubIntegration
{
    using MassTransit.Registration;
    using Pipeline;
    using Pipeline.Observables;


    public interface IEvenHubProducerProviderFactory
    {
        IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = null);
    }


    public class EvenHubProducerProviderFactory :
        IEvenHubProducerProviderFactory
    {
        readonly IBusInstance _busInstance;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IMessageSerializer _messageSerializer;
        readonly SendObservable _sendObservable;
        readonly ISendPipe _sendPipe;

        public EvenHubProducerProviderFactory(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance, ISendPipe sendPipe,
            SendObservable sendObservable, IMessageSerializer messageSerializer)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _sendPipe = sendPipe;
            _sendObservable = sendObservable;
            _messageSerializer = messageSerializer;
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = null)
        {
            return new EventHubProducerProvider(_hostConfiguration, _busInstance, _sendPipe, _sendObservable, _messageSerializer, consumeContext);
        }
    }
}
