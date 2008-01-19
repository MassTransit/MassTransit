/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.

using MassTransit.ServiceBus.Exceptions;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// An abstract factory to create an IMessageReceiver
    /// </summary>
    public class MessageReceiverFactory : IMessageReceiverFactory
    {
        /// <summary>
        /// Creates an instance of an object that implements IMessageReceiver appropriate for the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for receiving messages</param>
        /// <returns>An instance supporting IMessageReceiver</returns>
        public IMessageReceiver Using(IEndpoint endpoint)
        {
            if (endpoint is IMessageQueueEndpoint)
            {
                return new MessageQueueReceiver(endpoint as IMessageQueueEndpoint);
            }

            throw new EndpointException(endpoint, "No Message Receiver Available. The endpoint is not of type 'IMessageQueueEndpoint'");
        }
    }
}