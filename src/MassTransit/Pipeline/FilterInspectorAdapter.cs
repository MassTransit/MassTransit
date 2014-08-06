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
    using Magnum.Extensions;


    public class FilterInspectorAdapter<TFilter, TContext> :
        IFilterInspectorAdapter
        where TFilter : class, IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IFilterInspector<TFilter, TContext> _inspector;

        public FilterInspectorAdapter(IFilterInspector<TFilter, TContext> inspector)
        {
            _inspector = inspector;
        }

        public bool Inspect<T>(IFilter<T> filter, FilterInspectorCallback callback)
            where T : class, PipeContext
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            if (callback == null)
                throw new ArgumentNullException("callback");

            var actualFilter = filter as TFilter;
            if (actualFilter == null)
                throw new ArgumentException("Unexpected filter type: " + filter.GetType().ToShortTypeName());

            return _inspector.Inspect(actualFilter, callback);
        }
    }
}