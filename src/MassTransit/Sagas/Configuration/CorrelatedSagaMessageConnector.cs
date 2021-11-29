namespace MassTransit.Configuration
{
    using System;
    using Middleware;


    /// <summary>
    /// Connects a message that has an exact CorrelationId to the saga instance
    /// to the saga repository.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class CorrelatedSagaMessageConnector<TSaga, TMessage> :
        SagaMessageConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Func<ConsumeContext<TMessage>, Guid> _correlationIdSelector;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public CorrelatedSagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter, ISagaPolicy<TSaga, TMessage> policy,
            Func<ConsumeContext<TMessage>, Guid> correlationIdSelector)
            : base(consumeFilter)
        {
            _policy = policy;
            _correlationIdSelector = correlationIdSelector;
        }

        protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TSaga> repository,
            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe)
        {
            configurator.UseFilter(new CorrelationIdMessageFilter<TMessage>(_correlationIdSelector));
            configurator.UseFilter(new CorrelatedSagaFilter<TSaga, TMessage>(repository, _policy, sagaPipe));
        }
    }
}
