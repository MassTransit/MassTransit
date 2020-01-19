namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System.Linq;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public interface ILoadQueryProvider<out TSaga>
        where TSaga : class, ISaga
    {
        IQueryable<TSaga> GetQueryable(DbContext dbContext);
    }
}
