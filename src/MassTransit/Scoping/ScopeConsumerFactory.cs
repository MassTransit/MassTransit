namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using GreenPipes;


    public class ScopeConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerScopeProvider _scopeProvider;

        public ScopeConsumerFactory(IConsumerScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            using IConsumerScopeContext<TConsumer, TMessage> scope = _scopeProvider.GetScope<TConsumer, TMessage>(context);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<TConsumer>("scope");
            _scopeProvider.Probe(scope);
        }
    }
}
