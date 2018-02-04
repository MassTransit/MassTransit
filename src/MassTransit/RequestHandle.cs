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
namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// A request handle manages the client-side request, and allows the request to be configured, response types added, etc. The handle
    /// should be disposed once it is no longer in-use, and the request has been completed (successfully, or otherwise).
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    public interface RequestHandle<TRequest> :
        RequestHandle,
        IPipeConfigurator<SendContext<TRequest>>
        where TRequest : class
    {
        /// <summary>
        /// The request message that was/will be sent.
        /// </summary>
        Task<TRequest> Message { get; }
    }


    public interface RequestHandle :
        IDisposable
    {
        /// <summary>
        /// The RequestId assigned to the request, and used in the header for the outgoing request message
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// Set the request message time to live, which by default is equal to the request timeout. Clearing this value
        /// will prevent any TimeToLive value from being specified.
        /// </summary>
        RequestTimeout TimeToLive { set; }

        /// <summary>
        /// If the specified result type is present, it is returned.
        /// </summary>
        /// <param name="readyToSend">If true, sets the request as ready to send and sends it</param>
        /// <typeparam name="T">The result type</typeparam>
        /// <returns>True if the result type specified is present, otherwise false</returns>
        Task<Response<T>> GetResponse<T>(bool readyToSend = true)
            where T : class;

        /// <summary>
        /// Cancel the request
        /// </summary>
        void Cancel();
    }
}