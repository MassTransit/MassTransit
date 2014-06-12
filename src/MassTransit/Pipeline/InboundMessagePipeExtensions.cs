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
namespace MassTransit.Pipeline
{
    using System;
    using MassTransit.Configuration;
    using Sinks;
    using SubscriptionConnectors;


    public static class InboundMessagePipeExtensions
    {
        public static ConnectHandle ConnectHandler<T>(this IInboundMessagePipe pipe, MessageHandler<T> handler,
            IMessageRetryPolicy retryPolicy = null)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.Connect(pipe, handler, retryPolicy ?? Retry.None);
        }

        public static ConnectHandle ConnectConsumer<T>(this IInboundMessagePipe pipe,
            IAsyncConsumerFactory<T> consumerFactory, IMessageRetryPolicy retryPolicy = null)
            where T : class
        {
            return ConsumerConnectorCache<T>.Connector.Connect(pipe, consumerFactory, retryPolicy ?? Retry.None);
        }

        public static ConnectHandle ConnectConsumer<T>(this IInboundMessagePipe pipe,
            IMessageRetryPolicy retryPolicy = null)
            where T : class, new()
        {
            var consumerFactory = new DefaultConstructorAsyncConsumerFactory<T>();

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<T>();

            return connector.Connect(pipe, consumerFactory, retryPolicy ?? Retry.None);
        }

        public static ConnectHandle ConnectConsumer<T>(this IInboundMessagePipe pipe, Func<T> factoryMethod,
            IMessageRetryPolicy retryPolicy = null)
            where T : class
        {
            var consumerFactory = new DelegateAsyncConsumerFactory<T>(factoryMethod);

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<T>();

            return connector.Connect(pipe, consumerFactory, retryPolicy ?? Retry.None);
        }
    }
}