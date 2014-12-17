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
namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using PipeBuilders;
    using Pipeline;
    using Pipeline.Filters;


    public class TupleFilterBuilderConfigurator<TContext, TMessage> :
        IPipeBuilderConfigurator<TupleContext<TContext, TMessage>>
        where TContext : class, PipeContext
        where TMessage : class
    {
        readonly IFilter<TContext> _filter;

        public TupleFilterBuilderConfigurator(IFilter<TContext> filter)
        {
            _filter = filter;
        }

        public void Build(IPipeBuilder<TupleContext<TContext, TMessage>> builder)
        {
            builder.AddFilter(new TupleSplitFilter<TContext, TMessage>(_filter));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_filter == null)
                yield return this.Failure("Filter", "must not be null");
        }
    }
}