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
namespace MassTransit.Pipeline.Pipes
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Monitoring.Introspection;


    /// <summary>
    /// The last pipe in a pipeline is always an end pipe that does nothing and returns synchronously
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LastPipe<T> :
        IPipe<T>
        where T : class, PipeContext
    {
        readonly IFilter<T> _filter;

        public LastPipe(IFilter<T> filter)
        {
            _filter = filter;
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            return _filter.Probe(context);
        }

        [DebuggerNonUserCode]
        public Task Send(T context)
        {
            return _filter.Send(context, Cache.LastPipe);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return _filter.Visit(visitor);
        }


        static class Cache
        {
            internal static readonly IPipe<T> LastPipe = new Last();
        }


        class Last :
            IPipe<T>
        {
            async Task IProbeSite.Probe(ProbeContext context)
            {
                //            await _filter.Probe(context);
//                await _next.Probe(context);
            }

            async Task IPipe<T>.Send(T context)
            {
            }

            public bool Visit(IPipelineVisitor visitor)
            {
                return true;
            }
        }
    }
}