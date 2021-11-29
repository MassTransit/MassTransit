namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data.Entity;
    using System.Linq;


    public class CustomSagaLoadQueryProvider<TSaga> :
        ILoadQueryProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<IQueryable<TSaga>, IQueryable<TSaga>> _customize;
        readonly ILoadQueryProvider<TSaga> _source;

        public CustomSagaLoadQueryProvider(ILoadQueryProvider<TSaga> source, Func<IQueryable<TSaga>, IQueryable<TSaga>> customize)
        {
            _source = source;
            _customize = customize;
        }

        public IQueryable<TSaga> GetQueryable(DbContext dbContext)
        {
            return _customize(_source.GetQueryable(dbContext));
        }
    }
}
