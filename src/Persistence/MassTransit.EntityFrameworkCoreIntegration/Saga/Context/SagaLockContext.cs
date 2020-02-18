namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Context
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MassTransit.Saga;


    public interface SagaLockContext<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Execute the callback on each saga instance, and return a Task that waits on the results
        /// </summary>
        /// <returns></returns>
        Task<IList<TSaga>> Load();
    }
}
