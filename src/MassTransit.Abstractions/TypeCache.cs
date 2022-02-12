namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Internals;


    public static class TypeCache
    {
        static CachedType GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedType)Activator.CreateInstance(typeof(CachedType<>).MakeGenericType(type)));
        }

        internal static void GetOrAdd<T>(Type type, ITypeCache<T> typeCache)
        {
            Cached.Instance.GetOrAdd(type, _ => new CachedType<T>(typeCache));
        }

        public static string GetShortName(Type type)
        {
            return GetOrAdd(type).ShortName;
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedType> Instance = new ConcurrentDictionary<Type, CachedType>();
        }


        interface CachedType
        {
            string ShortName { get; }
        }


        class CachedType<T> :
            CachedType
        {
            string? _shortName;

            // ReSharper disable once UnusedMember.Local
            public CachedType()
            {
            }

            public CachedType(ITypeCache<T> typeCache)
            {
                _shortName = typeCache.ShortName;
            }

            public string ShortName => _shortName ??= TypeCache<T>.ShortName;
        }
    }


    public class TypeCache<T> :
        ITypeCache<T>
    {
        readonly Lazy<ReadOnlyPropertyCache<T>> _readPropertyCache;
        readonly string _shortName;
        readonly Lazy<ReadWritePropertyCache<T>> _writePropertyCache;

        TypeCache()
        {
            _shortName = typeof(T).GetTypeName();
            _readPropertyCache = new Lazy<ReadOnlyPropertyCache<T>>(() => new ReadOnlyPropertyCache<T>());
            _writePropertyCache = new Lazy<ReadWritePropertyCache<T>>(() => new ReadWritePropertyCache<T>());

            TypeCache.GetOrAdd(typeof(T), this);
        }

        public static IReadOnlyPropertyCache<T> ReadOnlyPropertyCache => Cached.Metadata.Value.ReadOnlyPropertyCache;
        public static IReadWritePropertyCache<T> ReadWritePropertyCache => Cached.Metadata.Value.ReadWritePropertyCache;

        public static string ShortName => Cached.Metadata.Value.ShortName;

        IReadOnlyPropertyCache<T> ITypeCache<T>.ReadOnlyPropertyCache => _readPropertyCache.Value;
        IReadWritePropertyCache<T> ITypeCache<T>.ReadWritePropertyCache => _writePropertyCache.Value;
        string ITypeCache<T>.ShortName => _shortName;


        static class Cached
        {
            internal static readonly Lazy<ITypeCache<T>> Metadata = new Lazy<ITypeCache<T>>(() => new TypeCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
