// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using GreenPipes;


    /// <summary>
    /// Configure a receiving AmazonSQS endpoint
    /// </summary>
    public interface IAmazonSqsReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IQueueEndpointConfigurator
    {
        /// <summary>
        /// The host on which the endpoint is being configured
        /// </summary>
        IAmazonSqsHost Host { get; }

        /// <summary>
        /// If true, creates message consumers for the message types in consumers, handlers, etc.
        /// With AmazonSQS, these are virtual consumers tied to the virtual topics
        /// </summary>
        bool BindMessageTopics { set; }

        /// <summary>
        /// Bind an existing exchange for the message type to the receive endpoint by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Bind<T>(Action<ITopicBindingConfigurator> callback = null)
            where T : class;

        /// <summary>
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="topicName">The exchange name</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Bind(string topicName, Action<ITopicBindingConfigurator> callback);

        void ConfigureSession(Action<IPipeConfigurator<ModelContext>> configure);
        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);
    }
}