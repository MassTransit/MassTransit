namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;

    public interface IRelationalEntityMetadataHelper
    {
        string GetTableName<T>(DbContext context) where T : class;
        string GetTableName(ObjectContext context, Type t);
    }
}
