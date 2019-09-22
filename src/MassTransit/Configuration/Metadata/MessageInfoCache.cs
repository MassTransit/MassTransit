namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Internals.GraphValidation;


    public class MessageInfoCache :
        IMessageInfoCache
    {
        readonly ConcurrentDictionary<Type, MessageInfo> _messageTypes;

        MessageInfoCache()
        {
            _messageTypes = new ConcurrentDictionary<Type, MessageInfo>();
        }

        MessageInfo IMessageInfoCache.GetMessageInfo<T>()
        {
            return _messageTypes.GetOrAdd(typeof(T), _ => CreateMessageInfo<T>());
        }

        public static MessageInfo GetMessageInfo(Type objectType)
        {
            return Cached.CachedTypes.GetOrAdd(objectType, (ICachedType)Activator.CreateInstance(typeof(CachedType<>).MakeGenericType(objectType)))
                .GetMessageInfo();
        }

        public static MessageInfo GetMessageInfo<T>()
            where T : class
        {
            return Cached.Instance.Value.GetMessageInfo<T>();
        }

        public static ObjectInfo[] GetMessageObjectInfo(params MessageInfo[] messageInfos)
        {
            var graph = new DependencyGraph<ObjectInfo>(messageInfos.Length);
            var seen = new HashSet<ObjectInfo>();

            void ApplyProperties(ObjectInfo sourceInfo)
            {
                if (seen.Contains(sourceInfo))
                    return;

                seen.Add(sourceInfo);
                graph.Add(sourceInfo);

                foreach (var propertyInfo in sourceInfo.Properties)
                {
                    if ((propertyInfo.Kind & PropertyKind.Object) == PropertyKind.Object)
                    {
                        if (ObjectInfoCache.TryGetObjectInfo(propertyInfo.PropertyType, out var objectInfo))
                        {
                            graph.Add(sourceInfo, objectInfo);

                            ApplyProperties(objectInfo);
                        }
                    }
                }
            }

            foreach (var messageInfo in messageInfos)
            {
                if (ObjectInfoCache.TryGetObjectInfo(messageInfo.ObjectType, out var objectInfo))
                    ApplyProperties(objectInfo);
            }

            return graph.GetItemsInOrder().ToArray();
        }

        static MessageInfo CreateMessageInfo<T>()
            where T : class
        {
            var objectInfo = ObjectInfoCache.GetObjectInfo<T>();

            return new CachedMessageInfo(objectInfo, TypeMetadataCache<T>.MessageTypeNames);
        }


        interface ICachedType
        {
            MessageInfo GetMessageInfo();
        }


        class CachedType<T> :
            ICachedType
            where T : class
        {
            public MessageInfo GetMessageInfo()
            {
                return Cached.Instance.Value.GetMessageInfo<T>();
            }
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageInfoCache> Instance = new Lazy<IMessageInfoCache>(() => new MessageInfoCache());
            internal static readonly ConcurrentDictionary<Type, ICachedType> CachedTypes = new ConcurrentDictionary<Type, ICachedType>();
        }


        class CachedMessageInfo :
            MessageInfo
        {
            readonly ObjectInfo _objectInfo;

            public CachedMessageInfo(ObjectInfo objectInfo, string[] messageTypes)
            {
                _objectInfo = objectInfo;
                MessageTypes = messageTypes;
            }

            public string ObjectType => _objectInfo.ObjectType;

            public string[] MessageTypes { get; }

            public PropertyInfo[] Properties => _objectInfo.Properties;
        }
    }
}
