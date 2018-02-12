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
    /// <summary>
    /// A supervisor with a set of agents (a supervisor is also an agent)
    /// </summary>
    public interface ISupervisor :
        IAgent
    {
        /// <summary>
        /// Add an Agent to the Supervisor
        /// </summary>
        /// <param name="agent">The agent</param>
        void Add(IAgent agent);

        /// <summary>
        /// The peak number of agents active at the same time
        /// </summary>
        int PeakActiveCount { get; }

        /// <summary>
        /// The total number of agents that were added to the supervisor
        /// </summary>
        long TotalCount { get; }
    }


    /// <summary>
    /// A supervisor that is also a <see cref="IPipeContextSource{TContext}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISupervisor<out T> :
        ISupervisor,
        IAgent<T>
        where T : class, PipeContext
    {
    }
}