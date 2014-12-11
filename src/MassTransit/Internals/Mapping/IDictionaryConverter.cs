namespace MassTransit.Internals.Mapping
{
    using System.Collections.Generic;


    public interface IDictionaryConverter
    {
        IDictionary<string, object> GetDictionary(object obj);
    }
}