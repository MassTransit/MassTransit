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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    /// <summary>
    /// The client context is used to access the queue/subscription/topic client.
    /// </summary>
    public interface ClientContext :
        PipeContext
    {
        /// <summary>
        /// The input address for the client/transport
        /// </summary>
        Uri InputAddress { get; }

        /// <summary>
        /// The path of the messaging entity
        /// </summary>
        string EntityPath { get; }

        /// <summary>
        /// Register a session handler factory asynchronously using the underlying client
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="exceptionHandler"></param>
        /// <returns></returns>
        Task RegisterSessionHandlerFactoryAsync(IMessageSessionAsyncHandlerFactory factory, EventHandler<ExceptionReceivedEventArgs> exceptionHandler);

        /// <summary>
        /// Register an message handler for the client
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="exceptionHandler"></param>
        void OnMessageAsync(Func<BrokeredMessage, Task> callback, EventHandler<ExceptionReceivedEventArgs> exceptionHandler);
    }
}