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
        readonly IPipe<ReceiveContext> _pipe;
        readonly IReceiveObserver _receiveObserver;
        readonly ReceiveSettings _settings;
        readonly ITaskSupervisor _supervisor;

        public HttpConsumerPipeSpecification(IPipe<ReceiveContext> pipe, ReceiveSettings settings, IReceiveObserver receiveObserver, IReceiveEndpointObserver endpointObserver, ITaskSupervisor supervisor)
        {
            _pipe = pipe;
            _settings = settings;
            _receiveObserver = receiveObserver;
            _endpointObserver = endpointObserver;
            _supervisor = supervisor;
        }

        public void Apply(IPipeBuilder<OwinHostContext> builder)
        {
            IPipe<OwinHostContext> pipe = Pipe.New<OwinHostContext>(x =>
            {
                x.UseFilter(new PrepareRouteFilter(_settings));

                x.UseFilter(new HttpConsumerFilter(_pipe, _receiveObserver, _endpointObserver, _supervisor));
            });

            IFilter<OwinHostContext> hostFilter = new HttpReceiveHostFilter(pipe, _supervisor, _settings);

            builder.AddFilter(hostFilter);
        }

        public IEnumerable<ValidationResult> Validate()
        {

            if (_settings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}