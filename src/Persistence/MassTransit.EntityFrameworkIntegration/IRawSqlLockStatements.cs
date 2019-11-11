namespace MassTransit.EntityFrameworkIntegration
{
    using System.Data.Entity;
    using MassTransit.Saga;


    public interface IRawSqlLockStatements
    {
        string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga;
    }
}
