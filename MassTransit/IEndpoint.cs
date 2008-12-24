// Copyright 2007-2008 The Apache Software Foundation.
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
        /// The address of the endpoint, in URI format
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Sends a message to the endpoint
        /// </summary>
        /// <typeparam name="T">The type of the message to send</typeparam>
        /// <param name="message">The message to send</param>
        void Send<T>(T message) where T : class;

        /// <summary>
        /// Sends a message to the endpoint
        /// </summary>
        /// <typeparam name="T">The type of the message to send</typeparam>
        /// <param name="message">The message to send</param>
        /// <param name="timeToLive">The maximum time for the message to be received before it expires</param>
        void Send<T>(T message, TimeSpan timeToLive) where T : class;

		/// <summary>
		/// Receive a message from an endpoint and dispatch it to the consumer pipeline
		/// </summary>
		/// <param name="timeout"></param>
		/// <param name="receiver">Func(the message, Func_To_Dequeue_the_Message(the message, did it work), did the dequeue work)</param>
		void Receive(TimeSpan timeout, Func<object, Func<object, bool>, bool> receiver);
    }
}