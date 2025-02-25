namespace MassTransit.AzureTable.Saga
{
    using System.Collections.Generic;


    public interface IEntityPropertyConverter<in TEntity>
        where TEntity : class
    {
        void ToEntity(TEntity entity, IDictionary<string, object> entityProperties);
        void FromEntity(TEntity entity, IDictionary<string, object> entityProperties);
    }
}
