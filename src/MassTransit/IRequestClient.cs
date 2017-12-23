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
namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// An abstraction that allows a message handler to respond to a request with all
    /// processing handled under the covers
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface IRequestClient<in TRequest, TResponse>
    {
        /// <summary>
        /// Send the request, and complete the response task when the response is received. If
        /// the request times out, a RequestTimeoutException is thrown. If the remote service 
        /// returns a fault, the task is set to exception status.
        /// </summary>
        /// <param name="request">The request message</param>
        /// <param name="cancellationToken">A cancellation token for the request</param>
        /// <returns>The response Task</returns>
        Task<TResponse> Request(TRequest request, CancellationToken cancellationToken = default(CancellationToken));
    }
}