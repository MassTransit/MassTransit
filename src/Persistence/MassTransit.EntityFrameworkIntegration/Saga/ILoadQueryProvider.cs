namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System.Data.Entity;
    using System.Linq;
    using MassTransit.Saga;


    public interface ILoadQueryProvider<out TSaga>
        where TSaga : class, ISaga
    {
        IQueryable<TSaga> GetQueryable(DbContext dbContext);
    }
}
