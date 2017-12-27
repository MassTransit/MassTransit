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
    using GreenPipes;
    using Integration;
    using Topology;
    using Util;


    public interface IRabbitMqHost :
        IHost
    {
        IConnectionCache ConnectionCache { get; }

        RabbitMqHostSettings Settings { get; }

        /// <summary>
        /// Returns the topology of the RabbitMQ host
        /// </summary>
        new IRabbitMqHostTopology Topology { get; }

        /// <summary>
        /// The connection retry policy used for connecting to the host
        /// </summary>
        IRetryPolicy ConnectionRetryPolicy { get; }

        /// <summary>
        /// The supervisor for the host, which indicates when it's being stopped
        /// </summary>
        ITaskSupervisor Supervisor { get; }

        /// <summary>
        /// Create a temporary receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(Action<IRabbitMqReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Create a receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configure = null);
    }
}