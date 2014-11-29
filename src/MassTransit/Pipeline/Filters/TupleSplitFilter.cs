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
namespace MassTransit.Pipeline.Filters
{
    using System.Threading.Tasks;
    using Pipes;


    /// <summary>
    /// Splits a context item off the pipe and carries it out-of-band to be merged
    /// once the next filter has completed
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class TupleSplitFilter<TContext, TMessage> :
        IFilter<TupleContext<TContext, TMessage>>
        where TMessage : class
        where TContext : class, PipeContext
    {
        readonly IFilter<TContext> _next;

        public TupleSplitFilter(IFilter<TContext> next)
        {
            _next = next;
        }

        public Task Send(TupleContext<TContext, TMessage> context, IPipe<TupleContext<TContext, TMessage>> next)
        {
            var mergePipe = new TupleMergePipe<TContext, TMessage>(context.Value, next);

            return _next.Send(context.Context, mergePipe);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, x => _next.Inspect(x) && _next.Inspect(x));
        }
    }
}