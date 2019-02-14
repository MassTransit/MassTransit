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
namespace MassTransit.ActiveMqTransport
{
    using System;
    using GreenPipes;
    using Topology;
    using Transport;


    public interface IActiveMqHost :
        IHost
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IRetryPolicy ConnectionRetryPolicy { get; }

        ActiveMqHostSettings Settings { get; }

        new IActiveMqHostTopology Topology { get; }

        /// <summary>
        /// Connect a receive endpoint for the host, using an endpoint definition
        /// </summary>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Create a receive endpoint on the host, with a separate handle for stopping/removing the endpoint
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configure = null);
    }
}
