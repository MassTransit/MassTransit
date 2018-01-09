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
namespace GreenPipes.Agents
{
    using System.Threading.Tasks;


    /// <summary>
    /// Completes the AsyncPipeContextAgent when the context is sent to the pipe, and doesn't return until the agent completes
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class AsyncPipeContextFilter<TContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IAsyncPipeContextAgent<TContext> _agent;

        public AsyncPipeContextFilter(IAsyncPipeContextAgent<TContext> agent)
        {
            _agent = agent;
        }

        public async Task Send(TContext context, IPipe<TContext> next)
        {
            await _agent.Created(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);

            await _agent.Completed.ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}