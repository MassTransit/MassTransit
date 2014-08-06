// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;


    /// <summary>
    /// Caches the converters that allow a raw object to be published using the object's type through
    /// the generic Send method.
    /// </summary>
    public class SendToEndpointConverterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<ISendToEndpointConverter>> _types =
            new ConcurrentDictionary<Type, Lazy<ISendToEndpointConverter>>();

        public static SendToEndpointConverterCache Instance
        {
            get { return InstanceCache.Cached.Value; }
        }

        public ISendToEndpointConverter this[Type type]
        {
            get { return _types.GetOrAdd(type, CreateTypeConverter).Value; }
        }

        static Lazy<ISendToEndpointConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<ISendToEndpointConverter>(() => CreateConverter(type));
        }

        static ISendToEndpointConverter CreateConverter(Type type)
        {
            Type converterType = typeof(SendToEndpointConverter<>).MakeGenericType(type);

            return (ISendToEndpointConverter)Activator.CreateInstance(converterType);
        }


        static class InstanceCache
        {
            internal static readonly Lazy<SendToEndpointConverterCache> Cached =
                new Lazy<SendToEndpointConverterCache>(() => new SendToEndpointConverterCache(), LazyThreadSafetyMode.PublicationOnly);
        }
    }
}