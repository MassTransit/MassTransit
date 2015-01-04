// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;


    public class ConsumerFilterVisitorAdapterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IConsumerFilterVisitorAdapter>> _types =
            new ConcurrentDictionary<Type, Lazy<IConsumerFilterVisitorAdapter>>();

        public static IConsumerFilterVisitorAdapter GetAdapter(Type[] types)
        {
            Type converterType = typeof(ConsumerFilterVisitorAdapter<,>).MakeGenericType(types);

            return Cached.Adapters.Value._types.GetOrAdd(converterType, CreateTypeConverter).Value;
        }

        static Lazy<IConsumerFilterVisitorAdapter> CreateTypeConverter(Type type)
        {
            return new Lazy<IConsumerFilterVisitorAdapter>(() => CreateConverter(type));
        }

        static IConsumerFilterVisitorAdapter CreateConverter(Type type)
        {
            return (IConsumerFilterVisitorAdapter)Activator.CreateInstance(type);
        }


        static class Cached
        {
            internal static readonly Lazy<ConsumerFilterVisitorAdapterCache> Adapters =
                new Lazy<ConsumerFilterVisitorAdapterCache>(() => new ConsumerFilterVisitorAdapterCache(),
                    LazyThreadSafetyMode.PublicationOnly);
        }
    }
}