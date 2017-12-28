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
namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.ComponentModel;
    using MassTransit.Builders;
    using Topology.Configuration;


    public interface IServiceBusBusFactoryConfigurator :
        IBusFactoryConfigurator,
        IServiceBusQueueEndpointConfigurator
    {
         IServiceBusSendTopologyConfigurator SendTopology { get; }

         IServiceBusPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// In most cases, this is not needed and should not be used. However, if for any reason the default bus
        /// endpoint queue name needs to be changed, this will do it. Do NOT set it to the same name as a receive
        /// endpoint or you will screw things up.
        /// </summary>
        void OverrideDefaultBusEndpointQueueName(string value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddBusFactorySpecification(IBusFactorySpecification<IBusBuilder> specification);

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddReceiveEndpointSpecification(IReceiveEndpointSpecification<IBusBuilder> specification);

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
        /// Before configuring any topology options, calling this will make it so that send and publish
        /// topologies are completely separated for this bus. This means that some types may not properly
        /// follow the topology rules, so use with caution.
        /// </summary>
        void SeparatePublishFromSendTopology();

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
        /// <param name="topicName">The topic name to subscribe</param>
        /// <param name="configure"></param>
        void SubscriptionEndpoint(IServiceBusHost host, string subscriptionName, string topicName, Action<IServiceBusSubscriptionEndpointConfigurator> configure);
    }
}