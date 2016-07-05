// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading;


    /// <summary>
    /// A cache of convention-based CorrelationId mappers, used unless overridden by some mystical force
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EndpointConventionCache<T> :
        IEndpointConventionCache<T>
        where T : class
    {
        readonly Lazy<EndpointAddressProvider<T>> _endpointAddressProvider;

        EndpointConventionCache()
        {
            _endpointAddressProvider = new Lazy<EndpointAddressProvider<T>>(CreateDefaultConvention);
        }

        EndpointConventionCache(EndpointAddressProvider<T> endpointAddressProvider)
        {
            _endpointAddressProvider = new Lazy<EndpointAddressProvider<T>>(() => endpointAddressProvider);
        }

        bool IEndpointConventionCache<T>.TryGetEndpointAddress(T message, out Uri address)
        {
            return _endpointAddressProvider.Value(message, out address);
        }

        internal static void Map(EndpointAddressProvider<T> endpointAddressProvider)
        {
            if (Cached.Metadata.IsValueCreated)
                throw new InvalidOperationException("The correlationId pipe has already been created");

            Cached.Metadata = new Lazy<IEndpointConventionCache<T>>(() => new EndpointConventionCache<T>(endpointAddressProvider));
        }

        internal static bool TryGetEndpointAddress(T message, out Uri address)
        {
            return Cached.Metadata.Value.TryGetEndpointAddress(message, out address);
        }

        EndpointAddressProvider<T> CreateDefaultConvention()
        {
            return (T message, out Uri address) =>
            {
                address = null;
                return false;
            };
        }


        static class Cached
        {
            internal static Lazy<IEndpointConventionCache<T>> Metadata = new Lazy<IEndpointConventionCache<T>>(
                () => new EndpointConventionCache<T>(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}