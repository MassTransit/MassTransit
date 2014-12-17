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
namespace MassTransit
{
    using System;
    using Context;
    using SubscriptionConfigurators;
    using SubscriptionConnectors;


    public static class HandlerSubscriptionExtensions
    {
        /// <summary>
        /// Subscribes a message handler to the receive endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="handler"></param>
        public static void Handler<T>(this IReceiveEndpointConfigurator configurator, MessageHandler<T> handler)
            where T : class
        {
            var handlerConfigurator = new HandlerConfigurator<T>(handler);

            configurator.AddConfigurator(handlerConfigurator);
        }

        public static void Handler<T>(this IReceiveEndpointConfigurator configurator, MessageHandler<T> handler,
            Action<IHandlerConfigurator<T>> configure)
            where T : class
        {
            var handlerConfigurator = new HandlerConfigurator<T>(handler);

            configure(handlerConfigurator);

            configurator.AddConfigurator(handlerConfigurator);
        }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
        public static ConnectHandle SubscribeHandler<T>(this IBus bus, MessageHandler<T> handler)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.Connect(bus.ConsumePipe, handler);
        }

        /// <summary>
        /// Subscribe a request handler to the bus's endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bus"></param>
        /// <param name="requestId"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ConnectHandle SubscribeRequestHandler<T>(this IBus bus, Guid requestId, MessageHandler<T> handler)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.Connect(bus.ConsumePipe, requestId, handler);
        }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
        public static ConnectHandle SubscribeHandler<T>(this IBus bus, Action<T> handler)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.Connect(bus.ConsumePipe, async context => handler(context.Message));
        }

        /// <summary>
        /// Adds a message handler to the service bus for handling a specific type of message
        /// </summary>
        /// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
        /// <param name="bus"></param>
        /// <param name="handler">The callback to invoke when messages of the specified type arrive on the service bus</param>
        public static ConnectHandle SubscribeContextHandler<T>(this IBus bus, Action<IConsumeContext<T>> handler)
            where T : class
        {
            return HandlerConnectorCache<T>.Connector.Connect(bus.ConsumePipe, async context =>
            {
                IConsumeContext<T> consumeContext = new ConsumeContextAdapter<T>(context);

                handler(consumeContext);
            });
        }
    }
}