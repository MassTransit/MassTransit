namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Internals;


    public static class TypeMetadataCache
    {
        static readonly object _builderLock = new object();
        public static IImplementationBuilder ImplementationBuilder => Cached.Builder;

        public static Type GetImplementationType(Type type)
        {
            lock (_builderLock)
                return Cached.Builder.GetImplementationType(type);
        }

        public static string GetShortName(Type type)
        {
            return TypeCache.GetShortName(type);
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return MessageTypeCache.GetProperties(type);
        }

        public static bool IsValidMessageType(Type type)
        {
            return MessageTypeCache.IsValidMessageType(type);
        }

        public static bool IsTemporaryMessageType(Type type)
        {
            return MessageTypeCache.IsTemporaryMessageType(type);
        }

        public static bool HasConsumerInterfaces(Type type)
        {
            return MessageTypeCache.HasConsumerInterfaces(type);
        }

        public static bool HasSagaInterfaces(Type type)
        {
            return MessageTypeCache.HasSagaInterfaces(type);
        }

        public static Type[] GetMessageTypes(Type type)
        {
            return MessageTypeCache.GetMessageTypes(type);
        }

        public static string[] GetMessageTypeNames(Type type)
        {
            return MessageTypeCache.GetMessageTypeNames(type);
        }


        static class Cached
        {
            internal static readonly IImplementationBuilder Builder = new DynamicImplementationBuilder();
        }


        public static bool IsValidMessageDataType(Type type)
        {
            return type.IsInterfaceOrConcreteClass() && MessageTypeCache.IsValidMessageType(type) && !type.IsValueTypeOrObject();
        }
    }


    public class TypeMetadataCache<T> :
        ITypeMetadataCache<T>
    {
        readonly Lazy<Type> _implementationType;

        TypeMetadataCache()
        {
            _implementationType = new Lazy<Type>(() => TypeMetadataCache.GetImplementationType(typeof(T)));
        }

        public static Type ImplementationType => Cached.Metadata.Value.ImplementationType;

        public static string ShortName => TypeCache<T>.ShortName;
        public static string DiagnosticAddress => MessageTypeCache<T>.DiagnosticAddress;
        public static bool HasSagaInterfaces => MessageTypeCache<T>.HasSagaInterfaces;
        public static bool HasConsumerInterfaces => MessageTypeCache<T>.HasConsumerInterfaces;
        public static IEnumerable<PropertyInfo> Properties => MessageTypeCache<T>.Properties;
        public static bool IsValidMessageType => MessageTypeCache<T>.IsValidMessageType;
        public static string InvalidMessageTypeReason => MessageTypeCache<T>.InvalidMessageTypeReason;
        public static bool IsTemporaryMessageType => MessageTypeCache<T>.IsTemporaryMessageType;
        public static Type[] MessageTypes => MessageTypeCache<T>.MessageTypes;
        public static string[] MessageTypeNames => MessageTypeCache<T>.MessageTypeNames;

        Type ITypeMetadataCache<T>.ImplementationType => _implementationType.Value;


        static class Cached
        {
            internal static readonly Lazy<ITypeMetadataCache<T>> Metadata = new Lazy<ITypeMetadataCache<T>>(
                () => new TypeMetadataCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
