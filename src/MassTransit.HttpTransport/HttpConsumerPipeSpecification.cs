namespace MassTransit.HttpTransport
{
    using System.Collections.Generic;
    using Configuration.Builders;
    using Configurators;
    using Hosting;
    using MassTransit.Pipeline;
    using PipeBuilders;
    using PipeConfigurators;
    using Util;


    public class HttpConsumerPipeSpecification :
        IPipeSpecification<OwinHostContext>
    {
        readonly IReceiveEndpointObserver _endpointObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IReceiveObserver _receiveObserver;
        readonly ReceiveSettings _settings;
        readonly ITaskSupervisor _supervisor;

        public HttpConsumerPipeSpecification(IPipe<ReceiveContext> receivePipe, ReceiveSettings settings, IReceiveObserver receiveObserver, IReceiveEndpointObserver endpointObserver, ITaskSupervisor supervisor)
        {
            _receivePipe = receivePipe;
            _settings = settings;
            _receiveObserver = receiveObserver;
            _endpointObserver = endpointObserver;
            _supervisor = supervisor;
        }

        public void Apply(IPipeBuilder<OwinHostContext> builder)
        {
            builder.AddFilter(new HttpConsumerFilter(_receivePipe, _receiveObserver, _endpointObserver, _supervisor));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}