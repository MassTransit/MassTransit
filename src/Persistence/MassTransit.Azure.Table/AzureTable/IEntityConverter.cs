namespace MassTransit.AzureTable
{
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;


    public interface IEntityConverter<T>
        where T : class
    {
        IDictionary<string, EntityProperty> GetDictionary(T entity);
        T GetObject(IDictionary<string, EntityProperty> entityProperties);
    }
}
