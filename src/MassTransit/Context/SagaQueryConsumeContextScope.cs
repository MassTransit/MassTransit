namespace MassTransit.Context
{
    using Saga;


    /// <summary>
    /// A consumer instance merged with a message consume context
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class SagaQueryConsumeContextScope<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaQueryConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly ISagaQuery<TSaga> _query;

        public SagaQueryConsumeContextScope(ConsumeContext<TMessage> context, ISagaQuery<TSaga> query)
            : base(context)
        {
            _query = query;
        }

        public SagaQueryConsumeContextScope(ConsumeContext<TMessage> context, ISagaQuery<TSaga> query, params object[] payloads)
            : base(context, payloads)
        {
            _query = query;
        }

        ISagaQuery<TSaga> SagaQueryConsumeContext<TSaga, TMessage>.Query => _query;
    }
}
