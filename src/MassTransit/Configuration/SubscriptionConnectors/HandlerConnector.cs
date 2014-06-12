// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.SubscriptionConnectors
{
    using System;
    using Pipeline;
    using Pipeline.Sinks;
    using Util;


    public interface HandlerConnector
    {
        ConnectHandle Connect<T>(IInboundMessagePipe pipe, MessageHandler<T> handler, IMessageRetryPolicy retryPolicy)
            where T : class;
    }


    public class HandlerConnector<TMessage> :
        HandlerConnector
        where TMessage : class
    {
        public ConnectHandle Connect<T>(IInboundMessagePipe pipe, MessageHandler<T> handler, IMessageRetryPolicy retryPolicy)
            where T : class
        {
            var messageHandler = handler as MessageHandler<TMessage>;
            if (messageHandler == null)
                throw new ArgumentException("The message handler type does not match: " + TypeMetadataCache<T>.ShortName);

            var messagePipe = new HandlerMessagePipe<TMessage>(messageHandler, retryPolicy);

            return pipe.Connect(messagePipe);
        }
    }
}