namespace MassTransit.AzureTable
{
    using System.Collections.Generic;


    public interface IEntityConverter<T>
        where T : class
    {
        IDictionary<string, object> GetDictionary(T entity);
        T GetObject(IDictionary<string, object> entityProperties);
    }
}
