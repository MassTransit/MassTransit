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


    public class ConsumeMessageFilterVisitorAdapterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IConsumeMessageFilterVisitorAdapter>> _types =
            new ConcurrentDictionary<Type, Lazy<IConsumeMessageFilterVisitorAdapter>>();

        public static IConsumeMessageFilterVisitorAdapter GetAdapter(Type[] types)
        {
            Type converterType = typeof(ConsumeMessageFilterVisitorAdapter<>).MakeGenericType(types);

            return Cached.Adapters.Value._types.GetOrAdd(converterType, CreateTypeConverter).Value;
        }

        static Lazy<IConsumeMessageFilterVisitorAdapter> CreateTypeConverter(Type type)
        {
            return new Lazy<IConsumeMessageFilterVisitorAdapter>(() => CreateConverter(type));
        }

        static IConsumeMessageFilterVisitorAdapter CreateConverter(Type type)
        {
            return (IConsumeMessageFilterVisitorAdapter)Activator.CreateInstance(type);
        }


        static class Cached
        {
            internal static readonly Lazy<ConsumeMessageFilterVisitorAdapterCache> Adapters =
                new Lazy<ConsumeMessageFilterVisitorAdapterCache>(() => new ConsumeMessageFilterVisitorAdapterCache(),
                    LazyThreadSafetyMode.PublicationOnly);
        }
    }
}