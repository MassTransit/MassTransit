namespace MassTransit.Configuration
{
    using Middleware;


    public partial class SagaConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        public class QuerySagaMessageConnector :
            SagaMessageConnector
        {
            readonly ISagaPolicy<TSaga, TMessage> _policy;
            readonly ISagaQueryFactory<TSaga, TMessage> _queryFactory;

            public QuerySagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter, ISagaPolicy<TSaga, TMessage> policy,
                ISagaQueryFactory<TSaga, TMessage> queryFactory)
                : base(consumeFilter)
            {
                _policy = policy;
                _queryFactory = queryFactory;
            }

            protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TSaga> repository,
                IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe)
            {
                configurator.UseFilter(new QuerySagaFilter<TSaga, TMessage>(repository, _policy, _queryFactory, sagaPipe));
            }
        }
    }
}
