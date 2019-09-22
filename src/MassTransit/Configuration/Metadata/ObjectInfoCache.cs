namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Concurrent;
    using Contracts;


    public class ObjectInfoCache :
        IObjectInfoCache
    {
        readonly ConcurrentDictionary<string, ObjectInfo> _messageTypes;

        ObjectInfoCache()
        {
            _messageTypes = new ConcurrentDictionary<string, ObjectInfo>();
        }

        ObjectInfo IObjectInfoCache.GetObjectInfo<T>()
        {
            return _messageTypes.GetOrAdd(MessageUrn.ForTypeString<T>(), _ => CreateObjectInfo<T>());
        }

        ObjectInfo IObjectInfoCache.GetOrAddObjectInfo(string messageType, ObjectInfo objectInfo)
        {
            return _messageTypes.GetOrAdd(messageType, objectInfo);
        }

        bool IObjectInfoCache.TryGetObjectInfo(string messageType, out ObjectInfo objectInfo)
        {
            return _messageTypes.TryGetValue(messageType, out objectInfo);
        }

        public static ObjectInfo GetObjectInfo(Type objectType)
        {
            return Cached.CachedTypes.GetOrAdd(objectType, (ICachedType)Activator.CreateInstance(typeof(CachedType<>).MakeGenericType(objectType)))
                .GetObjectInfo();
        }

        public static ObjectInfo GetObjectInfo<T>()
            where T : class
        {
            return Cached.Instance.Value.GetObjectInfo<T>();
        }

        public static ObjectInfo GetOrAddObjectInfo(string messageType, ObjectInfo objectInfo)
        {
            return Cached.Instance.Value.GetOrAddObjectInfo(messageType, objectInfo);
        }

        public static bool TryGetObjectInfo(string messageType, out ObjectInfo objectInfo)
        {
            return Cached.Instance.Value.TryGetObjectInfo(messageType, out objectInfo);
        }

        static ObjectInfo CreateObjectInfo<T>()
        {
            return new CachedObjectInfo(MessageUrn.ForTypeString<T>(), () => PropertyInfoCache<T>.Properties);
        }


        interface ICachedType
        {
            ObjectInfo GetObjectInfo();
        }


        class CachedType<T> :
            ICachedType
            where T : class
        {
            public ObjectInfo GetObjectInfo()
            {
                return Cached.Instance.Value.GetObjectInfo<T>();
            }
        }


        static class Cached
        {
            internal static readonly Lazy<IObjectInfoCache> Instance = new Lazy<IObjectInfoCache>(() => new ObjectInfoCache());
            internal static readonly ConcurrentDictionary<Type, ICachedType> CachedTypes = new ConcurrentDictionary<Type, ICachedType>();
        }


        class CachedObjectInfo :
            ObjectInfo
        {
            readonly Func<PropertyInfo[]> _getProperties;
            PropertyInfo[] _properties;

            public CachedObjectInfo(string objectType, Func<PropertyInfo[]> getProperties)
            {
                _getProperties = getProperties;
                ObjectType = objectType;
            }

            public string ObjectType { get; }

            public PropertyInfo[] Properties => _properties ?? (_properties = _getProperties());
        }
    }
}
