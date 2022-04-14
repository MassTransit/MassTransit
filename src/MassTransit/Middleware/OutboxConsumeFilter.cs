namespace MassTransit.Middleware
{
    using System.Threading.Tasks;
    using DependencyInjection;


    /// <summary>
    /// Sends the message through the outbox
    /// </summary>
    /// <typeparam name="TContext">The outbox context type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class OutboxConsumeFilter<TContext, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TContext : class
        where TMessage : class
    {
        readonly OutboxConsumeOptions _options;
        readonly IConsumeScopeProvider _scopeProvider;

        public OutboxConsumeFilter(IConsumeScopeProvider scopeProvider, OutboxConsumeOptions options)
        {
            _scopeProvider = scopeProvider;
            _options = options;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("outbox");
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            await using IConsumeScopeContext<TMessage> scope = await _scopeProvider.GetScope(context).ConfigureAwait(false);

            var contextFactory = scope.GetService<IOutboxContextFactory<TContext>>();
            if (contextFactory == null)
                throw new ConsumerException($"Unable to resolve outbox context factory for type '{TypeCache<TContext>.ShortName}'.");

            var pipe = new OutboxMessagePipe<TMessage>(next);

            await contextFactory.Send(scope.Context, _options, pipe).ConfigureAwait(false);
        }
    }
}
