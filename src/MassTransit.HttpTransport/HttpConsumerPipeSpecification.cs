namespace MassTransit.HttpTransport
{
    using System.Collections.Generic;
    using Configuration.Builders;
    using GreenPipes;
    using Hosting;
    using Util;


    public class HttpConsumerPipeSpecification :
        IPipeSpecification<OwinHostContext>
    {
        readonly IReceiveEndpointObserver _endpointObserver;
        readonly HttpHostSettings _settings;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IReceiveObserver _receiveObserver;
        readonly ITaskSupervisor _supervisor;

        public HttpConsumerPipeSpecification(HttpHostSettings settings, 
            IPipe<ReceiveContext> receivePipe, 
            IReceiveObserver receiveObserver,
            IReceiveEndpointObserver endpointObserver,
            ITaskSupervisor supervisor)
        {
            _settings = settings;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _endpointObserver = endpointObserver;
            _supervisor = supervisor;
        }

        public void Apply(IPipeBuilder<OwinHostContext> builder)
        {
            builder.AddFilter(new HttpConsumerFilter(_receivePipe, _receiveObserver, _endpointObserver, _supervisor, _settings));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}