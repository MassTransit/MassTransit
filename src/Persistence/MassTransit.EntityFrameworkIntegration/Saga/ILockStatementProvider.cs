namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System.Data.Entity;
    using MassTransit.Saga;


    public interface ILockStatementProvider
    {
        string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga;
    }
}
