namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;


    /// <summary>
    /// Creates a DbContext for the saga repository
    /// </summary>
    public interface ISagaDbContextFactory<out TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Create a standalone DbContext
        /// </summary>
        /// <returns></returns>
        DbContext Create();

        /// <summary>
        /// Create a scoped DbContext within the lifetime scope of the saga repository
        /// </summary>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbContext CreateScoped<T>(ConsumeContext<T> context)
            where T : class;

        /// <summary>
        /// Release the DbContext once it is no longer needed
        /// </summary>
        /// <param name="dbContext"></param>
        ValueTask ReleaseAsync(DbContext dbContext);
    }
}
