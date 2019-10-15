// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public static class HostConnectReceiveEndpointExtensions
    {
        /// <summary>
        /// Connect a response endpoint for the host
        /// </summary>
        /// <param name="connector">The host to connect</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        public static HostReceiveEndpointHandle ConnectResponseEndpoint(this IReceiveConnector connector, IEndpointNameFormatter endpointNameFormatter = null,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return connector.ConnectReceiveEndpoint(new ResponseEndpointDefinition(), endpointNameFormatter, configureEndpoint);
        }

        /// <summary>
        /// Connect an endpoint for the host
        /// </summary>
        /// <param name="connector">The host to connect</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        public static HostReceiveEndpointHandle ConnectReceiveEndpoint(this IReceiveConnector connector, Action<IReceiveEndpointConfigurator>
            configureEndpoint = null)
        {
            return connector.ConnectReceiveEndpoint(new TemporaryEndpointDefinition(), null, configureEndpoint);
        }
    }
}
