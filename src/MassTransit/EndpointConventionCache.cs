namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;


    public static class EndpointConventionCache
    {
        public static bool TryGetEndpointAddress<T>(out Uri address)
        {
            return GetOrAdd(typeof(T)).TryGetEndpointAddress(typeof(T), out address);
        }

        public static bool TryGetEndpointAddress(Type messageType, out Uri address)
        {
            return GetOrAdd(messageType).TryGetEndpointAddress(messageType, out address);
        }

        static CachedConvention GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConvention)Activator.CreateInstance(typeof(CachedConvention<>).MakeGenericType(type)));
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConvention> Instance = new ConcurrentDictionary<Type, CachedConvention>();
        }


        interface CachedConvention
        {
            bool TryGetEndpointAddress(Type messageType, out Uri address);
        }


        class CachedConvention<TMessage> :
            CachedConvention
            where TMessage : class
        {
            bool CachedConvention.TryGetEndpointAddress(Type messageType, out Uri address)
            {
                if (!typeof(TMessage).IsAssignableFrom(messageType))
                    throw new ArgumentException($"Message was not a valid type: {TypeCache<TMessage>.ShortName}", nameof(messageType));

                return EndpointConventionCache<TMessage>.TryGetEndpointAddress(out address);
            }
        }
    }


    /// <summary>
    /// A cache of convention-based CorrelationId mappers, used unless overridden by some mystical force
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class EndpointConventionCache<TMessage> :
        IEndpointConventionCache<TMessage>
        where TMessage : class
    {
        readonly Lazy<EndpointAddressProvider<TMessage>> _endpointAddressProvider;

        EndpointConventionCache()
        {
            _endpointAddressProvider = new Lazy<EndpointAddressProvider<TMessage>>(CreateDefaultConvention);
        }

        EndpointConventionCache(EndpointAddressProvider<TMessage> endpointAddressProvider)
        {
            _endpointAddressProvider = new Lazy<EndpointAddressProvider<TMessage>>(() => endpointAddressProvider);
        }

        bool IEndpointConventionCache<TMessage>.TryGetEndpointAddress(out Uri address)
        {
            return _endpointAddressProvider.Value(out address);
        }

        internal static void Map(EndpointAddressProvider<TMessage> endpointAddressProvider)
        {
            if (Cached.Metadata.IsValueCreated)
                throw new InvalidOperationException("The endpoint convention has already been created and can no longer be modified.");

            Cached.Metadata = new Lazy<IEndpointConventionCache<TMessage>>(() => new EndpointConventionCache<TMessage>(endpointAddressProvider));
        }

        internal static void Map(Uri destinationAddress)
        {
            if (Cached.Metadata.IsValueCreated)
            {
                if (TryGetEndpointAddress(out var address) && address != destinationAddress)
                    throw new InvalidOperationException("The endpoint convention has already been created and can no longer be modified.");
            }

            Cached.Metadata = new Lazy<IEndpointConventionCache<TMessage>>(() => new EndpointConventionCache<TMessage>((out Uri address) =>
            {
                address = destinationAddress;
                return true;
            }));
        }

        internal static bool TryGetEndpointAddress(out Uri address)
        {
            return Cached.Metadata.Value.TryGetEndpointAddress(out address);
        }

        EndpointAddressProvider<TMessage> CreateDefaultConvention()
        {
            IEndpointConventionCache<TMessage>[] implementedTypes = MessageTypeCache<TMessage>.MessageTypes
                .Where(x => x != typeof(TMessage))
                .Select(x => Activator.CreateInstance(typeof(TypeAdapter<>).MakeGenericType(typeof(TMessage), x)))
                .Cast<IEndpointConventionCache<TMessage>>()
                .ToArray();

            if (implementedTypes.Any())
            {
                return (out Uri address) =>
                {
                    for (var i = 0; i < implementedTypes.Length; i++)
                    {
                        if (implementedTypes[i].TryGetEndpointAddress(out address))
                            return true;
                    }

                    address = default;
                    return false;
                };
            }

            return (out Uri address) =>
            {
                address = default;
                return false;
            };
        }


        class TypeAdapter<TAdapter> :
            IEndpointConventionCache<TMessage>
            where TAdapter : class
        {
            bool IEndpointConventionCache<TMessage>.TryGetEndpointAddress(out Uri address)
            {
                if (typeof(TAdapter).IsAssignableFrom(typeof(TMessage)))
                    return EndpointConventionCache<TAdapter>.TryGetEndpointAddress(out address);

                address = default;
                return false;
            }
        }


        static class Cached
        {
            internal static Lazy<IEndpointConventionCache<TMessage>> Metadata = new Lazy<IEndpointConventionCache<TMessage>>(
                () => new EndpointConventionCache<TMessage>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}
