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
    using Magnum.Extensions;


    public interface IPipeInspector
    {
        bool Inspect<T>(IFilter<T> filter)
            where T : class, PipeContext;

        bool Inspect<T>(IFilter<T> filter, FilterInspectorCallback<T> callback)
            where T : class, PipeContext;

        bool Inspect<T>(IConsumeFilter<T> filter)
            where T : class;

        bool Inspect<T>(IConsumeFilter<T> filter, FilterInspectorCallback<ConsumeContext<T>> callback)
            where T : class;

        bool Inspect<T>(IPipe<T> pipe)
            where T : class, PipeContext;

        bool Inspect<T>(IPipe<T> pipe, PipeInspectorCallback<T> callback)
            where T : class, PipeContext;

        bool Inspect<T>(IConsumePipe<T> pipe)
            where T : class;

        bool Inspect<T>(IConsumePipe<T> pipe, PipeInspectorCallback<ConsumeContext<T>> callback)
            where T : class;
    }


    public interface IFilterConverter
    {
        bool Inspect<T>(IFilter<T> filter, FilterInspectorCallback<T> callback)
            where T : class, PipeContext;
    }


    public class FilterConverter<TFilter> :
        IFilterConverter
        where TFilter : class
    {
        public bool Inspect<T>(IFilter<T> filter, FilterInspectorCallback<T> callback)
            where T : class, PipeContext
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            if (callback == null)
                throw new ArgumentNullException("callback");

            var actualFilter = filter as TFilter;
            if (actualFilter == null)
                throw new ArgumentException("Unexpected filter type: " + filter.GetType().ToShortTypeName());

            return true;
        }
    }


    public class PipeInspectorFilterCache
    {
        readonly ConcurrentDictionary<Type, Lazy<IFilterConverter>> _types =
            new ConcurrentDictionary<Type, Lazy<IFilterConverter>>();

        public static PipeInspectorFilterCache Instance
        {
            get { return InstanceCache.Cached.Value; }
        }

        public IFilterConverter this[Type type]
        {
            get { return _types.GetOrAdd(type, CreateTypeConverter).Value; }
        }

        static Lazy<IFilterConverter> CreateTypeConverter(Type type)
        {
            return new Lazy<IFilterConverter>(() => CreateConverter(type));
        }

        static IFilterConverter CreateConverter(Type type)
        {
            Type converterType = typeof(FilterConverter<>).MakeGenericType(type);

            return (IFilterConverter)Activator.CreateInstance(converterType);
        }


        static class InstanceCache
        {
            internal static readonly Lazy<PipeInspectorFilterCache> Cached =
                new Lazy<PipeInspectorFilterCache>(() => new PipeInspectorFilterCache(),
                    LazyThreadSafetyMode.PublicationOnly);
        }
    }
}