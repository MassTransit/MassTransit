namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Saga;
    using Util;


    /// <summary>
    /// A new saga that was created
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class NewSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxy<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        public NewSagaConsumeContext(ConsumeContext<TMessage> context, TSaga instance)
            : base(context)
        {
            Saga = instance;
        }

        public TSaga Saga { get; }
        public override Guid? CorrelationId => ((SagaConsumeContext<TSaga>)this).Saga.CorrelationId;

        public Task SetCompleted()
        {
            IsCompleted = true;

            return TaskUtil.Completed;
        }

        public bool IsCompleted { get; private set; }
    }
}
