namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Saga;


    public class SagaList<TSaga> :
        MessageList<ISagaInstance<TSaga>>,
        ISagaList<TSaga>
        where TSaga : class, ISaga
    {
        public SagaList(TimeSpan timeout)
            : base((int)timeout.TotalMilliseconds)
        {
        }

        public IEnumerable<ISagaInstance<TSaga>> Select(Func<TSaga, bool> filter)
        {
            return base.Select(x => filter(x.Saga));
        }

        public TSaga Contains(Guid sagaId)
        {
            return Select(x => x.Saga.CorrelationId == sagaId).Select(x => x.Saga).FirstOrDefault();
        }

        public void Add(SagaConsumeContext<TSaga> context)
        {
            Add(new SagaInstance<TSaga>(context.Saga), context.Saga.CorrelationId);
        }
    }
}
