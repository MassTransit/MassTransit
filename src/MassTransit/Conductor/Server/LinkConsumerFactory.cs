namespace MassTransit.Conductor.Server
{
    using System.Threading.Tasks;
    using Consumers;
    using Context;
    using Contracts;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;


    public class LinkConsumerFactory<TMessage> :
        IConsumerFactory<LinkConsumer<TMessage>>
        where TMessage : class
    {
        readonly IServiceEndpointMessageClientCache _clientCache;
        readonly Task<InstanceInfo> _instanceInfo;
        readonly Task<ServiceInfo> _serviceInfo;

        public LinkConsumerFactory(IServiceEndpoint serviceEndpoint, IServiceEndpointMessageClientCache clientCache)
        {
            _clientCache = clientCache;

            _instanceInfo = serviceEndpoint.InstanceInfo;
            _serviceInfo = serviceEndpoint.ServiceInfo;
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<LinkConsumer<TMessage>, T>> next)
            where T : class
        {
            var instanceInfo = _instanceInfo.IsCompletedSuccessfully() ? _instanceInfo.Result : await _instanceInfo.ConfigureAwait(false);
            var serviceInfo = _serviceInfo.IsCompletedSuccessfully() ? _serviceInfo.Result : await _serviceInfo.ConfigureAwait(false);

            var consumer = new LinkConsumer<TMessage>(_clientCache, serviceInfo, instanceInfo);

            await next.Send(new ConsumerConsumeContextScope<LinkConsumer<TMessage>, T>(context, consumer)).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<LinkConsumer<TMessage>>("serviceEndpoint");
        }
    }
}
