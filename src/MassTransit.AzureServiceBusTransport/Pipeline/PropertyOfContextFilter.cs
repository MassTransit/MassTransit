// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Allows a filter using a property of a context
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class PropertyOfContextFilter<TContext, TProperty> :
        IFilter<TContext>
        where TContext : class, PipeContext
        where TProperty : class, PipeContext
    {
        readonly Func<TContext, TProperty> _contextAccessor;
        readonly IFilter<TProperty> _propertyFilter;

        public PropertyOfContextFilter(IFilter<TProperty> propertyFilter, Func<TContext, TProperty> contextAccessor)
        {
            _propertyFilter = propertyFilter;
            _contextAccessor = contextAccessor;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            var propertyContext = _contextAccessor(context);

            await _propertyFilter.Send(propertyContext, Pipe.Empty<TProperty>()).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            _propertyFilter.Probe(context);
        }
    }
}