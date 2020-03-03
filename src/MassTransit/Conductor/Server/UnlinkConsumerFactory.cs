namespace MassTransit.Conductor.Server
{
    using System.Threading.Tasks;
    using Consumers;
    using Context;
    using GreenPipes;


    public class UnlinkConsumerFactory<TMessage> :
        IConsumerFactory<UnlinkConsumer<TMessage>>
        where TMessage : class
    {
        readonly IServiceEndpointMessageClientCache _clientCache;

        public UnlinkConsumerFactory(IServiceEndpointMessageClientCache clientCache)
        {
            _clientCache = clientCache;
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<UnlinkConsumer<TMessage>, T>> next)
            where T : class
        {
            var consumer = new UnlinkConsumer<TMessage>(_clientCache);

            await next.Send(new ConsumerConsumeContextScope<UnlinkConsumer<TMessage>, T>(context, consumer)).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<LinkConsumer<TMessage>>("serviceEndpoint");
        }
    }
}
