// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Threading.Tasks;


    /// <summary>
    ///     A Task-based request that is designed to be used by the TPL
    /// </summary>
    public interface ITaskRequest<out TRequest> :
        IRequest<TRequest>
        where TRequest : class
    {
        /// <summary>
        ///     The Task for the entire request, is completed when any of the response
        ///     handlers completes.
        /// </summary>
        Task Task { get; }

        /// <summary>
        ///     Cancel the request (will signal the Tasks as Cancelled)
        /// </summary>
        void Cancel();

        /// <summary>
        ///     Returns the task for the response handler of the specified message type
        /// </summary>
        /// <typeparam name="T">The response message type</typeparam>
        /// <returns>The Task associated with the response handler</returns>
        Task<T> GetResponseTask<T>()
            where T : class;

        /// <summary>
        ///     Returns the task for the response handler of the specified message type
        /// </summary>
        /// <param name="responseType">The response message type</param>
        /// <returns>The Task associated with the response handler</returns>
        Task GetResponseTask(Type responseType);
    }
}