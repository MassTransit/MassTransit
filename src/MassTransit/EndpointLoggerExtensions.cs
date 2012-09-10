// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;

    public static class EndpointLoggerExtensions
    {
        static readonly ILog _messages = Logger.Get("MassTransit.Messages");

        public static void LogSkipped(this IEndpointAddress sourceAddress, string messageId)
        {
            if (_messages.IsDebugEnabled)
            {
                _messages.DebugFormat("SKIP:{0}:{1}", sourceAddress, messageId);
            }
        }

        /// <summary>
        /// Log that a message was moved from one endpoint to the destination endpoint address
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="messageId"></param>
        /// <param name="description"> </param>
        public static void LogMoved(this IEndpointAddress sourceAddress, IEndpointAddress destinationAddress,
                                    string messageId, string description)
        {
            if (_messages.IsInfoEnabled)
            {
                _messages.InfoFormat("MOVE:{0}:{1}:{2}:{3}", sourceAddress, destinationAddress, messageId, description);
            }
        }

        /// <summary>
        /// Log that a message was requeued to the transport after an exception occurred
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <param name="destinationAddress"></param>
        /// <param name="messageId"></param>
        /// <param name="description"> </param>
        public static void LogReQueued(this IEndpointAddress sourceAddress, IEndpointAddress destinationAddress,
                                    string messageId, string description)
        {
            if (_messages.IsInfoEnabled)
            {
                _messages.InfoFormat("RQUE:{0}:{1}:{2}:{3}", sourceAddress, destinationAddress, messageId, description);
            }
        }

        public static void LogReceived(this IEndpointAddress sourceAddress, string messageId, string description)
        {
            if (_messages.IsDebugEnabled)
            {
                _messages.DebugFormat("RECV:{0}:{1}:{2}", sourceAddress, messageId, description);
            }
        }

        public static void LogSent(this IEndpointAddress address, string messageId, string description)
        {
            if (_messages.IsDebugEnabled)
            {
                _messages.DebugFormat("SEND:{0}:{1}:{2}", address, messageId, description);
            }
        }
    }
}