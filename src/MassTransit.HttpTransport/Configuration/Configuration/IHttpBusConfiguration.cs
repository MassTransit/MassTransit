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
namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Transport;


    public interface IHttpBusConfiguration :
        IBusConfiguration
    {
        /// <summary>
        /// Returns all the hosts
        /// </summary>
        new IHttpHostConfiguration[] Hosts { get; }

        /// <summary>
        /// Return the host associated with the specified address, if present.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        bool TryGetHost(Uri address, out IHttpHostConfiguration host);

        /// <summary>
        /// Return the host if it is part of the configuration
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        /// <param name="hostConfiguration"></param>
        bool TryGetHost(IHttpHost host, out IHttpHostConfiguration hostConfiguration);

        /// <summary>
        /// Create a host configuration, by adding a host to the bus
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        IHttpHostConfiguration CreateHostConfiguration(HttpHost host);

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IHttpEndpointConfiguration CreateEndpointConfiguration();

        /// <summary>
        /// Create a receive endpoint configuration for the default host
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="endpointConfiguration"></param>
        /// <returns></returns>
        IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, IHttpEndpointConfiguration endpointConfiguration);
    }
}