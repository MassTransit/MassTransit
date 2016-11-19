namespace MassTransit.HttpTransport
{
    using System.Collections.Generic;
    using Configuration.Builders;
    using GreenPipes;
    using Hosting;
    using MassTransit.Pipeline;
    using Util;


    public class HttpConsumerPipeSpecification :
        IPipeSpecification<OwinHostContext>
    {
        readonly IReceiveTransportObserver _transportObserver;
        readonly HttpHostSettings _settings;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IReceiveObserver _receiveObserver;
        readonly ITaskSupervisor _supervisor;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly IMessageSerializer _messageSerializer;
        readonly ISendPipe _sendPipe;

        public HttpConsumerPipeSpecification(HttpHostSettings settings, 
            IPipe<ReceiveContext> receivePipe, 
            IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver,
            ITaskSupervisor supervisor, 
            ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider,
            IMessageSerializer messageSerializer,
            ISendPipe sendPipe)
        {
            _settings = settings;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _messageSerializer = messageSerializer;
            _sendPipe = sendPipe;
        }

        public void Apply(IPipeBuilder<OwinHostContext> builder)
        {
            builder.AddFilter(new HttpConsumerFilter(_receivePipe, _receiveObserver, _transportObserver, _supervisor, _settings, _sendEndpointProvider,
                    _publishEndpointProvider, _messageSerializer, _sendPipe));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}