namespace MassTransit.EntityFrameworkIntegration
{
    using MassTransit.Saga;
    using System.Data.Entity;

    public interface IRawSqlLockStatements
    {
        string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga;
    }
}
