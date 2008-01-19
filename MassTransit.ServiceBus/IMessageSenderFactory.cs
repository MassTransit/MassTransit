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

namespace MassTransit.ServiceBus
{
    using System;

    public interface IMessageSenderFactory
    {
        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        IMessageSender Using(IEndpoint endpoint);

        /// <summary>
        /// Using an IMessageSender using the specified endpoint
        /// </summary>
        /// <param name="endpoint">The endpoint for sending messages</param>
        /// <returns>An instance that supports IMessageSender</returns>
        IMessageSender Using(Uri endpoint);
    }
}