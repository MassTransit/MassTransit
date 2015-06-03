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
namespace MassTransit
{
    using System;
    using RabbitMqTransport;


    public interface IRabbitMqBusFactoryConfigurator :
        IBusFactoryConfigurator,
        IQueueConfigurator
    {
        // change this to return an IRabbitMqHost 
        IRabbitMqHost Host(RabbitMqHostSettings settings);

//        /// <summary>
//        ///     Intercept the publishing of a message type that is assignable to T
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="callback"></param>
//        void OnPublish<T>(Action<RabbitMqPublishContext<T>> callback)
//            where T : class;
//
//        void OnPublish(Action<RabbitMqPublishContext> callback);
//
        // TODO change receive endpoint to use an IRabbitMqHost, so that we don't have to match 

        /// <summary>
        /// Declare a ReceiveEndpoint on the broker and configure the endpoint settings and message consumers.
        /// </summary>
        /// <param name="host">The host for this endpoint</param>
        /// <param name="queueName">The input queue name</param>
        /// <param name="configure">The configuration method</param>
        void ReceiveEndpoint(IRabbitMqHost host, string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure);
    }
}