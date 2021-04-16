using System;
using System.Threading.Tasks;
using MassTransit.Configuration;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.EventStoreDbIntegration.Serializers;
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
        readonly IHeadersSerializer _headersSerializer;
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly IMessageSerializer _messageSerializer;
        readonly SendObservable _sendObservable;
        readonly ISendPipe _sendPipe;

        public EventStoreDbProducerProvider(IEventStoreDbHostConfiguration hostConfiguration, IBusInstance busInstance, ISendPipe sendPipe,
            SendObservable sendObservable, IHeadersSerializer headersSerializer, IMessageSerializer messageSerializer)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _sendPipe = sendPipe;
            _sendObservable = sendObservable;
            _headersSerializer = headersSerializer;
            _messageSerializer = messageSerializer;
        }

        public Task<IEventStoreDbProducer> GetProducer(Uri address)
        {
            var context = new EventStoreDbTransportContext(_hostConfiguration, _sendPipe, _busInstance.HostConfiguration, address, _headersSerializer,
                _messageSerializer);

            if (_sendObservable.Count > 0)
                context.ConnectSendObserver(_sendObservable);

            IEventStoreDbProducer producer = new EventStoreDbProducer(context);
            return Task.FromResult(producer);
        }


        class EventStoreDbTransportContext :
            BaseSendTransportContext,
            EventStoreDbSendTransportContext
        {
            readonly Recycle<IProducerContextSupervisor> _producerContextSupervisor;

            public EventStoreDbTransportContext(IEventStoreDbHostConfiguration hostConfiguration, ISendPipe sendPipe,
                IHostConfiguration configuration, Uri endpointAddress, IHeadersSerializer headersSerializer,
                IMessageSerializer messageSerializer)
                : base(configuration)
            {
                HostAddress = configuration.HostAddress;
                SendPipe = sendPipe;
                EndpointAddress = new EventStoreDbEndpointAddress(HostAddress, endpointAddress);
                _producerContextSupervisor =
                    new Recycle<IProducerContextSupervisor>(() =>
                        new ProducerContextSupervisor(hostConfiguration.ConnectionContextSupervisor, headersSerializer, messageSerializer));
            }

            public Uri HostAddress { get; }

            public EventStoreDbEndpointAddress EndpointAddress { get; }

            public ISendPipe SendPipe { get; }

            public IProducerContextSupervisor ProducerContextSupervisor => _producerContextSupervisor.Supervisor;
        }
    }
}
