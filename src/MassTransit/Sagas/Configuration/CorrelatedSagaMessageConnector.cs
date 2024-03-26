namespace MassTransit.Configuration
{
    using System;
    using Middleware;


    public partial class SagaConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        /// <summary>
        /// Connects a message that has an exact CorrelationId to the saga instance
        /// to the saga repository.
        /// </summary>
        public class CorrelatedSagaMessageConnector :
            SagaMessageConnector
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
}
