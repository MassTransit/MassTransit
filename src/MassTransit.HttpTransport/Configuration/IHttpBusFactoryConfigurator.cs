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
namespace MassTransit.HttpTransport
{
    using System;
    using Hosting;


    public interface IHttpBusFactoryConfigurator :
        IBusFactoryConfigurator
    {
        IHttpHost Host(HttpHostSettings settings);

        /// <summary>
        /// Maps a handler to the path specified
        /// </summary>
        /// <param name="configure">Configures the receive endpoint on this path</param>
        void ReceiveEndpoint(Action<IHttpReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Maps a handler to the path specified
        /// </summary>
        /// <param name="host">The host on which to connect the path/handler</param>
        /// <param name="pathMatch">The path to match for this handler</param>
        /// <param name="configure">Configures the receive endpoint on this path</param>
        void ReceiveEndpoint(IHttpHost host, string pathMatch, Action<IHttpReceiveEndpointConfigurator> configure = null);
    }
}