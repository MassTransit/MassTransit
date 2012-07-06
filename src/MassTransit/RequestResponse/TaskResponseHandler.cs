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
namespace MassTransit.RequestResponse
{
#if NET40
    using System.Threading.Tasks;

    /// <summary>
    /// A task response handler provides access to the task itself
    /// </summary>
    public interface TaskResponseHandler :
        ResponseHandler
    {
        /// <summary>
        /// Returns the Task for the response handler
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// Returns the Task&lt;<typeparam name="T">T</typeparam>&gt; for the response handler
        /// </summary>
        Task<T> GetTask<T>()
            where T : class;

        /// <summary>
        /// Notifies the handler that the request has timed out.
        /// </summary>
        void HandleTimeout();
    }

    public interface TaskResponseHandler<T> :
        TaskResponseHandler
        where T : class
    {
        /// <summary>
        /// Returns the Task&lt;<typeparam name="T">T</typeparam>&gt; for the response handler
        /// </summary>
        new Task<T> Task { get; }
    }
#endif
}