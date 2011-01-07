// Copyright 2007-2010 The Apache Software Foundation.
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

    /// <summary>
    /// IEndpoint is implemented by an endpoint. An endpoint is an addressable location on the network.
    /// </summary>
    public interface IEndpoint :
        IDisposable
    {
        /// <summary>
        /// The address of the endpoint
        /// </summary>
        IEndpointAddress Address { get; }

        /// <summary>
        /// The URI of the endpoint
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Send to the endpoint
        /// </summary>
        /// <typeparam name="T">The type of the message to send</typeparam>
        /// <param name="message">The message to send</param>
        void Send<T>(T message)
            where T : class;

        /// <summary>
        /// Receive from the endpoint by passing a function that can preview the message and if the caller
        /// chooses to accept it, return a method that will consume the message.
        /// 
        /// Returns after the specified timeout if no message is available.
        /// </summary>
        /// <param name="receiver">The function to preview/consume the message</param>
        /// <param name="timeout">The time to wait for a message to be available</param>
        void Receive(Func<object, Action<object>> receiver, TimeSpan timeout);
    }
}