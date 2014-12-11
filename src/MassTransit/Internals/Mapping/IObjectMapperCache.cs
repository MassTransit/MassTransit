namespace MassTransit.Internals.Mapping
{
    using System;


    public interface IObjectMapperCache
    {
        IObjectConverter GetObjectConverter(Type type);
        IDictionaryConverter GetDictionaryConverter(Type type);
    }
}