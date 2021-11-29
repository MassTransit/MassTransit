namespace MassTransit.EntityFrameworkIntegration
{
    using System.Data.Entity;
    using System.Linq;


    public interface ILoadQueryProvider<out TSaga>
        where TSaga : class, ISaga
    {
        IQueryable<TSaga> GetQueryable(DbContext dbContext);
    }
}
