namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;

    public interface IRelationalEntityMetadataHelper
    {
        string GetTableName<T>(DbContext context) where T : class;
    }
}
