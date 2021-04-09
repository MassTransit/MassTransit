using System;
using System.Threading.Tasks;
using MassTransit.Configuration;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.Pipeline;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;
using MassTransit.Util;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbProducerProvider :
        IEventStoreDbProducerProvider
    {
        readonly IBusInstance _busInstance;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly IMessageSerializer _messageSerializer;
        readonly SendObservable _sendObservable;
        readonly ISendPipe _sendPipe;

        public EventStoreDbProducerProvider(IEventStoreDbHostConfiguration hostConfiguration, IBusInstance busInstance, ISendPipe sendPipe,
            SendObservable sendObservable, IMessageSerializer messageSerializer)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _sendPipe = sendPipe;
            _sendObservable = sendObservable;
            _messageSerializer = messageSerializer;
        }

        public Task<IEventStoreDbProducer> GetProducer(Uri address)
        {
            var context = new EventStoreDbTransportContext(_hostConfiguration, _sendPipe, _busInstance.HostConfiguration, address, _messageSerializer);

            if (_sendObservable.Count > 0)
                context.ConnectSendObserver(_sendObservable);

            IEventStoreDbProducer esdbProducer = new EventStoreDbProducer(context);
            return Task.FromResult(esdbProducer);
        }


        class EventStoreDbTransportContext :
            BaseSendTransportContext,
            EventStoreDbSendTransportContext
        {
            readonly Recycle<IProducerContextSupervisor> _producerContextSupervisor;

            public EventStoreDbTransportContext(IEventStoreDbHostConfiguration hostConfiguration, ISendPipe sendPipe,
                IHostConfiguration configuration, Uri endpointAddress, IMessageSerializer messageSerializer)
                : base(configuration)
            {
                HostAddress = configuration.HostAddress;
                SendPipe = sendPipe;
                EndpointAddress = new EventStoreDbEndpointAddress(HostAddress, endpointAddress);
                _producerContextSupervisor =
                    new Recycle<IProducerContextSupervisor>(() =>
                        new ProducerContextSupervisor(hostConfiguration.ConnectionContextSupervisor, EndpointAddress.StreamCategory, messageSerializer));
            }

            public Uri HostAddress { get; }

            public EventStoreDbEndpointAddress EndpointAddress { get; }

            public ISendPipe SendPipe { get; }

            public IProducerContextSupervisor ProducerContextSupervisor => _producerContextSupervisor.Supervisor;
        }
    }
}
