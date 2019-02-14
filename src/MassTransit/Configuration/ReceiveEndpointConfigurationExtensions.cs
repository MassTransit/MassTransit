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
namespace MassTransit
{
    using System;
    using Definition;
    using EndpointConfigurators;


    public static class ReceiveEndpointConfigurationExtensions
    {
        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void ReceiveEndpoint(this IBusFactoryConfigurator configurator, Action<IReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(new TemporaryEndpointDefinition(), DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void ReceiveEndpoint(this IBusFactoryConfigurator configurator, IEndpointDefinition definition, Action<IReceiveEndpointConfigurator>
            configure = null)
        {
            configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IManagementEndpointConfigurator ManagementEndpoint(this IBusFactoryConfigurator configurator,
            Action<IReceiveEndpointConfigurator> configure = null)
        {
            IReceiveEndpointConfigurator specification = null;
            configurator.ReceiveEndpoint(new ManagementEndpointDefinition(), DefaultEndpointNameFormatter.Instance, x =>
            {
                specification = x;

                configure?.Invoke(specification);
            });

            return new ManagementEndpointConfigurator(specification);
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IManagementEndpointConfigurator ManagementEndpoint(this IBusFactoryConfigurator configurator, IHost host,
            Action<IReceiveEndpointConfigurator> configure = null)
        {
            return ManagementEndpoint(configurator, configure);
        }

        /// <summary>
        /// Creates a management endpoint which can be used by controllable filters on a bus instance
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IManagementEndpointConfigurator ManagementEndpoint(this IHost host, Action<IReceiveEndpointConfigurator> configure = null)
        {
            IReceiveEndpointConfigurator specification = null;
            host.ConnectReceiveEndpoint(new ManagementEndpointDefinition(), DefaultEndpointNameFormatter.Instance, x =>
            {
                specification = x;

                configure?.Invoke(specification);
            });

            return new ManagementEndpointConfigurator(specification);
        }
    }
}
