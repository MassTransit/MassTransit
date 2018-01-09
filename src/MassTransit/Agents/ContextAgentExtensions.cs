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
namespace GreenPipes
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Payloads;


    public static class ContextAgentExtensions
    {
        /// <summary>
        /// Stop the agent, using the default StopContext
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task Stop(this IAgent agent, CancellationToken cancellationToken = default(CancellationToken))
        {
            var stopContext = new DefaultStopContext(cancellationToken)
            {
                Reason = "Stopped"
            };

            return agent.Stop(stopContext);
        }

        /// <summary>
        /// Stop the agent, using the default StopContext
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="reason">The reason for stopping the agent</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task Stop(this IAgent agent, string reason, CancellationToken cancellationToken = default(CancellationToken))
        {
            var stopContext = new DefaultStopContext(cancellationToken)
            {
                Reason = reason
            };

            return agent.Stop(stopContext);
        }


        class DefaultStopContext :
            BasePipeContext,
            StopContext
        {
            public DefaultStopContext(CancellationToken cancellationToken)
                : base(new PayloadCache(), cancellationToken)
            {
            }

            public string Reason { get; set; }
        }
    }
}