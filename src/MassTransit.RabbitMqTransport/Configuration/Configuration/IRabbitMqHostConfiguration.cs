// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using MassTransit.Configuration;
    using Topology;
    using Transport;
    using Transports;


    public interface IRabbitMqHostConfiguration :
        IHostConfiguration
    {
        IRabbitMqBusConfiguration BusConfiguration { get; }

        string Description { get; }

        new IRabbitMqHostControl Host { get; }

        /// <summary>
        /// Create a receive endpoint configuration using the specified host
        /// </summary>
        /// <returns></returns>
        IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName);

        new IRabbitMqHostTopology Topology { get; }

        /// <summary>
        /// True if the broker is confirming published messages
        /// </summary>
        bool PublisherConfirmation { get; }

        RabbitMqHostSettings Settings { get; }

        /// <summary>
        /// Create a model context supervisor so that channels can be created from the broker
        /// </summary>
        /// <returns></returns>
        IModelContextSupervisor CreateModelContextSupervisor();

        ISendTransport CreateSendTransport(IModelContextSupervisor modelContextSupervisor, IFilter<ModelContext> modelFilter,
            string exchangeName);

        Task<ISendTransport> CreatePublishTransport<T>()
            where T : class;
    }
}