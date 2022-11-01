namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using MassTransit.Configuration;
    using Transports;
    using Util;


    class EventHubProducerSendTransportContext :
        BaseSendTransportContext,
        EventHubSendTransportContext
    {
        readonly IHostConfiguration _configuration;
        readonly Recycle<IProducerContextSupervisor> _producerContextSupervisor;

        public EventHubProducerSendTransportContext(IEventHubHostConfiguration hostConfiguration, ISendPipe sendPipe,
            IHostConfiguration configuration, Uri endpointAddress, ISerialization serialization)
            : base(configuration, serialization)
        {
            _configuration = configuration;
            SendPipe = sendPipe;

            EndpointAddress = new EventHubEndpointAddress(HostAddress, endpointAddress);

            _producerContextSupervisor = new Recycle<IProducerContextSupervisor>(() =>
                new ProducerContextSupervisor(hostConfiguration.ConnectionContextSupervisor, EndpointAddress.EventHubName, serialization));
        }

        public Uri HostAddress => _configuration.HostAddress;

        public EventHubEndpointAddress EndpointAddress { get; }

        public ISendPipe SendPipe { get; }

        public Task Send(IPipe<ProducerContext> pipe, CancellationToken cancellationToken)
        {
            var supervisor = _producerContextSupervisor.Supervisor;

            return _configuration.Retry(() => supervisor.Send(pipe, cancellationToken), cancellationToken, supervisor.SendStopping);
        }

        public override string EntityName => EndpointAddress.EventHubName;
        public override string ActivitySystem => "event-hub";

        public override Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedByDesignException("Event Hub is a producer, not an outbox compatible transport");
        }
    }
}
