namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Contexts;
    using MassTransit.Registration;
    using Pipeline;
    using Pipeline.Observables;
    using Util;


    public class EventHubProducerProvider :
        IEventHubProducerProvider
    {
        readonly IBusInstance _busInstance;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IMessageSerializer _messageSerializer;
        readonly SendObservable _sendObservable;
        readonly ISendPipe _sendPipe;

        public EventHubProducerProvider(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance, ISendPipe sendPipe,
            SendObservable sendObservable, IMessageSerializer messageSerializer)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _sendPipe = sendPipe;
            _sendObservable = sendObservable;
            _messageSerializer = messageSerializer;
        }

        public Task<IEventHubProducer> GetProducer(Uri address)
        {
            var context = new EventHubTransportContext(_hostConfiguration, _sendPipe, _busInstance.HostConfiguration, address, _messageSerializer);

            if (_sendObservable.Count > 0)
                context.ConnectSendObserver(_sendObservable);

            IEventHubProducer eventHubProducer = new EventHubProducer(context);
            return Task.FromResult(eventHubProducer);
        }


        class EventHubTransportContext :
            BaseSendTransportContext,
            EventHubSendTransportContext
        {
            readonly Recycle<IProducerContextSupervisor> _producerContextSupervisor;

            public EventHubTransportContext(IEventHubHostConfiguration hostConfiguration, ISendPipe sendPipe,
                IHostConfiguration configuration, Uri endpointAddress, IMessageSerializer messageSerializer)
                : base(configuration)
            {
                HostAddress = configuration.HostAddress;
                SendPipe = sendPipe;
                EndpointAddress = new EventHubEndpointAddress(HostAddress, endpointAddress);
                _producerContextSupervisor =
                    new Recycle<IProducerContextSupervisor>(() =>
                        new ProducerContextSupervisor(hostConfiguration.ConnectionContextSupervisor, EndpointAddress.EventHubName, messageSerializer));
            }

            public Uri HostAddress { get; }

            public EventHubEndpointAddress EndpointAddress { get; }

            public ISendPipe SendPipe { get; }

            public IProducerContextSupervisor ProducerContextSupervisor => _producerContextSupervisor.Supervisor;
        }
    }
}
