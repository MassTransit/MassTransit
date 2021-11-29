namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System.Data.Entity;
    using System.Linq;


    public class DefaultSagaLoadQueryProvider<TSaga> :
        ILoadQueryProvider<TSaga>
        where TSaga : class, ISaga
    {
        public IQueryable<TSaga> GetQueryable(DbContext dbContext)
        {
            return dbContext.Set<TSaga>();
        }
    }
}
