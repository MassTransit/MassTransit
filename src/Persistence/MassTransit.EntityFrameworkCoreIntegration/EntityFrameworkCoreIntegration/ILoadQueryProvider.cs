namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;


    public interface ILoadQueryProvider<out TSaga>
        where TSaga : class, ISaga
    {
        IQueryable<TSaga> GetQueryable(DbContext dbContext);
    }
}
