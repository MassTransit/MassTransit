namespace MassTransit.AzureTable.Saga
{
    using System.Collections.Generic;
    using Azure.Data.Tables;


    public interface IEntityPropertyConverter<in TEntity>
        where TEntity : class
    {
        void ToEntity(TEntity entity, TableEntity tableEntity);
        void FromEntity(TEntity entity, IDictionary<string, object> entityProperties);
    }
}
