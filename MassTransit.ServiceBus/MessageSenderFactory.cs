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

using System;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// An abstract factory to create an IMessageSender
    /// </summary>
    public class MessageSenderFactory : IMessageSenderFactory
    {
        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        public IMessageSender Using(IEndpoint endpoint)
        {
            Check.Require(endpoint is IMessageQueueEndpoint,
                          string.Format("Endpoint: {0} - is not of type {1} ", endpoint.Uri,
                                        typeof (IMessageQueueEndpoint).FullName));

            return new MessageQueueSender(endpoint as IMessageQueueEndpoint);
        }

        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        public IMessageSender Using(Uri endpoint)
        {
            return new MessageQueueSender(new MessageQueueEndpoint(endpoint));
        }
    }
}