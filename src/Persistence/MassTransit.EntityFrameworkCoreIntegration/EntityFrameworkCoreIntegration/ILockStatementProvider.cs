namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;


    public interface ILockStatementProvider
    {
        string GetRowLockStatement<T>(DbContext context)
            where T : class;

        /// <summary>
        /// Returns the lock statement for the specified property (usable for any set)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="propertyNames">One or more property names</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetRowLockStatement<T>(DbContext context, params string[] propertyNames)
            where T : class;

        string GetOutboxStatement(DbContext context);

        string GetBulkOutboxStatement(DbContext context, int size);
    }
}
