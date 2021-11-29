namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;


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
