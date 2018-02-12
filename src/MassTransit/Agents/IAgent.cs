// Copyright 2012-2018 Chris Patterson
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
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// An agent can be supervised, and signals when it has completed
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// A Task which can be awaited and is completed when the agent is either ready or faulted/canceled
        /// </summary>
        Task Ready { get; }

        /// <summary>
        /// A Task which is completed when the agent has completed (should never be set to Faulted, per convention)
        /// </summary>
        Task Completed { get; }

        /// <summary>
        /// The token which indicates if the agent is in the process of stopping (or stopped)
        /// </summary>
        CancellationToken Stopping { get; }

        /// <summary>
        /// The token which indicates if the agent is stopped
        /// </summary>
        CancellationToken Stopped { get; }

        /// <summary>
        /// Stop the agent, and any supervised agents under it's control. Any faults related to stopping should
        /// be returned via this method, and not propogated to the <see cref="Completed"/> Task.
        /// </summary>
        /// <param name="context">The stop context</param>
        Task Stop(StopContext context);
    }


    /// <summary>
    /// An agent that is also a pipe context source, of the specified context type
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IAgent<out TContext> :
        IAgent,
        IPipeContextSource<TContext>
        where TContext : class, PipeContext
    {
    }
}