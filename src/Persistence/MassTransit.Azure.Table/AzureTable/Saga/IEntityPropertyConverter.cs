namespace MassTransit.AzureTable.Saga
{
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;


    public interface IEntityPropertyConverter<in TEntity>
        where TEntity : class
    {
        void ToEntity(TEntity entity, IDictionary<string, EntityProperty> entityProperties);
        void FromEntity(TEntity entity, IDictionary<string, EntityProperty> entityProperties);
    }
}
