// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using Saga;
    using Util;


    public static class EndpointConventionCache
    {
        public static bool TryGetEndpointAddress(object message, out Uri address)
        {
            var messageType = message.GetType();

            return GetOrAdd(messageType).TryGetEndpointAddress(message, out address);
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
            bool TryGetEndpointAddress(object message, out Uri address);
        }


        class CachedConvention<TMessage> :
            CachedConvention
            where TMessage : class
        {
            bool CachedConvention.TryGetEndpointAddress(object message, out Uri address)
            {
                var messageOfT = message as TMessage;
                if (messageOfT == null)
                    throw new ArgumentException($"Message was not a valid type: {TypeMetadataCache<TMessage>.ShortName}", nameof(message));

                return EndpointConventionCache<TMessage>.TryGetEndpointAddress(messageOfT, out address);
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

        bool IEndpointConventionCache<TMessage>.TryGetEndpointAddress(TMessage message, out Uri address)
        {
            return _endpointAddressProvider.Value(message, out address);
        }

        internal static void Map(EndpointAddressProvider<TMessage> endpointAddressProvider)
        {
            if (Cached.Metadata.IsValueCreated)
                throw new InvalidOperationException("The correlationId pipe has already been created");

            Cached.Metadata = new Lazy<IEndpointConventionCache<TMessage>>(() => new EndpointConventionCache<TMessage>(endpointAddressProvider));
        }

        internal static bool TryGetEndpointAddress(TMessage message, out Uri address)
        {
            return Cached.Metadata.Value.TryGetEndpointAddress(message, out address);
        }

        EndpointAddressProvider<TMessage> CreateDefaultConvention()
        {
            IEndpointConventionCache<TMessage>[] implementedTypes = TypeMetadataCache<TMessage>.MessageTypes
                .Where(x => x != typeof(TMessage))
                .Select(x => Activator.CreateInstance(typeof(TypeAdapter<>).MakeGenericType(typeof(TMessage), x)))
                .Cast<IEndpointConventionCache<TMessage>>()
                .ToArray();

            if (implementedTypes.Any())
            {
                return (TMessage message, out Uri address) =>
                {
                    for (var i = 0; i < implementedTypes.Length; i++)
                    {
                        if (implementedTypes[i].TryGetEndpointAddress(message, out address))
                            return true;
                    }

                    address = default(Uri);
                    return false;
                };
            }

            return (TMessage message, out Uri address) =>
            {
                address = default(Uri);
                return false;
            };
        }


        class TypeAdapter<TAdapter> :
            IEndpointConventionCache<TMessage>
            where TAdapter : class
        {
            bool IEndpointConventionCache<TMessage>.TryGetEndpointAddress(TMessage message, out Uri address)
            {
                var messageOfT = message as TAdapter;
                if (messageOfT != null)
                {
                    return EndpointConventionCache<TAdapter>.TryGetEndpointAddress(messageOfT, out address);
                }

                address = default(Uri);
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