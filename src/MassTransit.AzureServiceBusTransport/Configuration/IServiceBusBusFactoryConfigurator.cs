// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Topology.Configuration;


    public interface IServiceBusBusFactoryConfigurator :
        IBusFactoryConfigurator<IServiceBusReceiveEndpointConfigurator>,
        IReceiveConfigurator<IServiceBusHost, IServiceBusReceiveEndpointConfigurator>,
        IServiceBusQueueEndpointConfigurator
    {
        new IServiceBusSendTopologyConfigurator SendTopology { get; }

        new IServiceBusPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Set to true if the topology should be deployed only
        /// </summary>
        bool DeployTopologyOnly { set; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Send<T>(Action<IServiceBusMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IServiceBusMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// In most cases, this is not needed and should not be used. However, if for any reason the default bus
        /// endpoint queue name needs to be changed, this will do it. Do NOT set it to the same name as a receive
        /// endpoint or you will screw things up.
        /// </summary>
        void OverrideDefaultBusEndpointQueueName(string value);

        /// <summary>
        /// Configures a host
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        IServiceBusHost Host(ServiceBusHostSettings settings);

        /// <summary>
        /// Declare a ReceiveEndpoint on the broker and configure the endpoint settings and message consumers.
        /// </summary>
        /// <param name="host">The host for this endpoint</param>
        /// <param name="queueName">The input queue name</param>
        /// <param name="configure">The configuration method</param>
        void ReceiveEndpoint(IServiceBusHost host, string queueName, Action<IServiceBusReceiveEndpointConfigurator> configure);

        /// <summary>
        /// Specify a receive endpoint for the bus, using an endpoint definition
        /// </summary>
        /// <param name="host"></param>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(IServiceBusHost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Declare a subscription endpoint on the broker and configure the endpoint settings and message consumers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="subscriptionName"></param>
        /// <param name="configure"></param>
        void SubscriptionEndpoint<T>(IServiceBusHost host, string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class;

        /// <summary>
        /// Declare a subscription endpoint on the broker and configure the endpoint settings and message consumers
        /// </summary>
        /// <param name="host">The host for this endpoint</param>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// <param name="topicPath">The topic name to subscribe</param>
        /// <param name="configure"></param>
        void SubscriptionEndpoint(IServiceBusHost host, string subscriptionName, string topicPath,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure);
    }
}
