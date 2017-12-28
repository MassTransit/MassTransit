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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.ComponentModel;
    using MassTransit.Builders;
    using Topology.Configuration;


    public interface IRabbitMqBusFactoryConfigurator :
        IBusFactoryConfigurator,
        IQueueEndpointConfigurator
    {
         IRabbitMqSendTopologyConfigurator SendTopology { get; }

         IRabbitMqPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Send<T>(Action<IRabbitMqMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IRabbitMqMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Before configuring any topology options, calling this will make it so that send and publish
        /// topologies are completely separated for this bus. This means that some types may not properly
        /// follow the topology rules, so use with caution.
        /// </summary>
        void SeparatePublishFromSendTopology();

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddBusFactorySpecification(IBusFactorySpecification<IBusBuilder> specification);

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddReceiveEndpointSpecification(IReceiveEndpointSpecification<IBusBuilder> specification);

        /// <summary>
        /// In most cases, this is not needed and should not be used. However, if for any reason the default bus
        /// endpoint queue name needs to be changed, this will do it. Do NOT set it to the same name as a receive
        /// endpoint or you will screw things up.
        /// </summary>
        void OverrideDefaultBusEndpointQueueName(string value);

        /// <summary>
        /// Configure a Host that can be connected. If only one host is specified, it is used as the default
        /// host for receive endpoints.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        IRabbitMqHost Host(RabbitMqHostSettings settings);

        /// <summary>
        /// Create a temporary queue name, using the configured consume topology
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        string CreateTemporaryQueueName(string prefix);

        /// <summary>
        /// Declare a ReceiveEndpoint on the broker and configure the endpoint settings and message consumers.
        /// </summary>
        /// <param name="host">The host for this endpoint</param>
        /// <param name="queueName">The input queue name</param>
        /// <param name="configure">The configuration method</param>
        void ReceiveEndpoint(IRabbitMqHost host, string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure);
    }
}