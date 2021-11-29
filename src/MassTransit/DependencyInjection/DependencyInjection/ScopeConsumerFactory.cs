namespace MassTransit.DependencyInjection
{
    using System.Threading.Tasks;


    public class ScopeConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumeScopeProvider _scopeProvider;

        public ScopeConsumerFactory(IConsumeScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            await using IConsumerConsumeScopeContext<TConsumer, TMessage> scope = await _scopeProvider.GetScope<TConsumer, TMessage>(context);

            await next.Send(scope.Context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<TConsumer>("scope");
            _scopeProvider.Probe(scope);
        }
    }
}
