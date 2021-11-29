namespace MassTransit.Saga
{
    using System;
    using System.Linq.Expressions;


    public class SagaQuery<TSaga> :
        ISagaQuery<TSaga>
        where TSaga : class, ISaga
    {
        readonly Lazy<Func<TSaga, bool>> _filter;

        public SagaQuery(Expression<Func<TSaga, bool>> filterExpression)
        {
            FilterExpression = filterExpression;
            _filter = new Lazy<Func<TSaga, bool>>(filterExpression.Compile);
        }

        public Func<TSaga, bool> GetFilter()
        {
            return _filter.Value;
        }

        public Expression<Func<TSaga, bool>> FilterExpression { get; }
    }
}
