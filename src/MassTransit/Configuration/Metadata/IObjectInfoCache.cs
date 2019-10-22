namespace MassTransit.Metadata
{
    using Contracts;


    public interface IObjectInfoCache
    {
        ObjectInfo GetObjectInfo<T>()
            where T : class;

        ObjectInfo GetOrAddObjectInfo(string messageType, ObjectInfo objectInfo);

        bool TryGetObjectInfo(string messageType, out ObjectInfo objectInfo);
    }
}
