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
namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Used to observe the events signaled by a receive endpoint
    /// </summary>
    public interface IReceiveEndpointObserver
    {
        /// <summary>
        /// Called when the receive endpoint is ready to receive messages
        /// </summary>
        /// <param name="ready"></param>
        /// <returns></returns>
        Task Ready(ReceiveEndpointReady ready);

        /// <summary>
        /// Called when the receive endpoint is being stopped, prior to actually stopping
        /// </summary>
        /// <param name="stopping"></param>
        /// <returns></returns>
        Task Stopping(ReceiveEndpointStopping stopping);

        /// <summary>
        /// Called when the receive endpoint has completed
        /// </summary>
        /// <param name="completed"></param>
        /// <returns></returns>
        Task Completed(ReceiveEndpointCompleted completed);

        /// <summary>
        /// Called when the receive endpoint faults
        /// </summary>
        /// <param name="faulted"></param>
        /// <returns></returns>
        Task Faulted(ReceiveEndpointFaulted faulted);
    }
}
