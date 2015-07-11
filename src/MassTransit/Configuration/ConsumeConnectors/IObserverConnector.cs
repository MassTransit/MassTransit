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
namespace MassTransit.ConsumeConnectors
{
    using System;
    using Pipeline;


    /// <summary>
    /// Connects a message handler to the ConsumePipe
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IObserverConnector<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Connect a message handler for all messages of type T 
        /// </summary>
        /// <param name="consumePipe"></param>
        /// <param name="observer"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        ConnectHandle ConnectObserver(IConsumePipeConnector consumePipe, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters);

        /// <summary>
        /// Connect a message handler for messages with the specified RequestId
        /// </summary>
        /// <param name="consumePipe"></param>
        /// <param name="requestId"></param>
        /// <param name="observer"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        ConnectHandle ConnectRequestObserver(IRequestPipeConnector consumePipe, Guid requestId, IObserver<ConsumeContext<TMessage>> observer,
            params IFilter<ConsumeContext<TMessage>>[] filters);
    }
}