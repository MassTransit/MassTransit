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
namespace MassTransit.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;


    public class FilterInspectorAdapterCache<TInspector>
    {
        readonly ConcurrentDictionary<Type, Lazy<IFilterInspectorAdapter>> _types =
            new ConcurrentDictionary<Type, Lazy<IFilterInspectorAdapter>>();

        public static FilterInspectorAdapterCache<TInspector> Instance
        {
            get { return InstanceCache.Cached.Value; }
        }

        public IFilterInspectorAdapter this[Type type]
        {
            get { return _types.GetOrAdd(type, CreateTypeConverter).Value; }
        }

        static Lazy<IFilterInspectorAdapter> CreateTypeConverter(Type type)
        {
            return new Lazy<IFilterInspectorAdapter>(() => CreateConverter(type));
        }

        static IFilterInspectorAdapter CreateConverter(Type type)
        {
            Type converterType = typeof(FilterInspectorAdapter<,>).MakeGenericType(type);

            return (IFilterInspectorAdapter)Activator.CreateInstance(converterType);
        }


        static class InstanceCache
        {
            internal static readonly Lazy<FilterInspectorAdapterCache<TInspector>> Cached =
                new Lazy<FilterInspectorAdapterCache<TInspector>>(() => new FilterInspectorAdapterCache<TInspector>(),
                    LazyThreadSafetyMode.PublicationOnly);
        }
    }
}