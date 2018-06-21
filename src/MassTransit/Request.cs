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
    using System.Threading.Tasks;


    /// <summary>
    /// A sent request, that may be in-process until the request task is completed
    /// </summary>
    /// <typeparam name="TRequest">The request message type</typeparam>
    public interface Request<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// An awaitable Task that is completed when the request is completed, or faulted
        /// in the case of an error or timeout
        /// </summary>
        Task<TRequest> Task { get; }
    }
}