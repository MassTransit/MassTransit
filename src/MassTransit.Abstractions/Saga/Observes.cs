namespace MassTransit
{
    using System;
    using System.Linq.Expressions;


    public interface Observes<TMessage, TSaga> :
        IConsumer<TMessage>
        where TMessage : class
    {
        Expression<Func<TSaga, TMessage, bool>> CorrelationExpression { get; }
    }
}
