namespace MassTransit.EntityFrameworkCoreIntegration
{
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public interface ILockStatementProvider
    {
        string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga;
    }
}
