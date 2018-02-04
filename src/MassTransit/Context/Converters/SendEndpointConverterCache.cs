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
namespace MassTransit.Context.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class SendEndpointConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<ISendEndpointConverter>> _types = new ConcurrentDictionary<Type, Lazy<ISendEndpointConverter>>();

        ISendEndpointConverter this[Type type] => _types.GetOrAdd(type, CreateTypeConverter).Value;

        public static Task Send(ISendEndpoint endpoint, object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Send(endpoint, message, cancellationToken);
        }

        public static Task Send(ISendEndpoint endpoint, object message, Type messageType, IPipe<SendContext> pipe,
            CancellationToken cancellationToken = default)
        {
            return Cached.Converters.Value[messageType].Send(endpoint, message, pipe, cancellationToken);
        }

        static Lazy<ISendEndpointConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<ISendEndpointConverter>(() => CreateConverter(type));
        }

        static ISendEndpointConverter CreateConverter(Type type)
        {
            var converterType = typeof(SendEndpointConverter<>).MakeGenericType(type);

            return (ISendEndpointConverter)Activator.CreateInstance(converterType);
        }


        static class Cached
        {
            internal static readonly Lazy<SendEndpointConverterCache> Converters =
                new Lazy<SendEndpointConverterCache>(() => new SendEndpointConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}