// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Returned once a bus has been started. Should call Stop or Dispose before the process
    /// can exit.
    /// </summary>
    public interface BusHandle
    {
        /// <summary>
        /// A task which can be awaited to know when the bus is ready and all of the receive endpoints have reported ready.
        /// </summary>
        Task<ReceiveEndpointReady[]> Ready { get; }

        /// <summary>
        /// Stop the bus and all receiving endpoints on the bus. Note that cancelling the Stop
        /// operation may leave the bus and/or one or more receive endpoints in an indeterminate
        /// state.
        /// </summary>
        /// <param name="cancellationToken">Cancel the stop operation in progress</param>
        /// <returns>An awaitable task that is completed once everything is stopped</returns>
        Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}