namespace MassTransit.EntityFrameworkIntegration
{
    using System.Data.Entity;


    public interface ILockStatementProvider
    {
        string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga;
    }
}
