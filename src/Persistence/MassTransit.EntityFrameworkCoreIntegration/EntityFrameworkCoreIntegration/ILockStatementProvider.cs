namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;


    public interface ILockStatementProvider
    {
        string GetRowLockStatement<T>(DbContext context)
            where T : class;
    }
}
