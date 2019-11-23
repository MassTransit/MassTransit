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
namespace MassTransit
{
    using System;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Configurators;
    using Azure.ServiceBus.Core.Contexts;
    using Definition;


    public static class ServiceBusBusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Adds a service bus host using the MassTransit style URI host name
        /// </summary>
        /// <param name="configurator">The bus factory configurator</param>
        /// <param name="hostAddress">The host address, in MassTransit format (sb://namespace.servicebus.windows.net/scope)</param>
        /// <param name="configure">A callback to further configure the service bus</param>
        /// <returns>The service bus host</returns>
        public static IServiceBusHost Host(this IServiceBusBusFactoryConfigurator configurator, Uri hostAddress,
            Action<IServiceBusHostConfigurator> configure)
        {
            var hostConfigurator = new ServiceBusHostConfigurator(hostAddress);

            configure(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        /// <summary>
        /// Adds a Service Bus host using a connection string (Endpoint=...., etc.).
        /// </summary>
        /// <param name="configurator">The bus factory configurator</param>
        /// <param name="connectionString">The connection string in the proper format</param>
        /// <param name="configure">A callback to further configure the service bus</param>
        /// <returns>The service bus host</returns>
        public static IServiceBusHost Host(this IServiceBusBusFactoryConfigurator configurator, string connectionString,
            Action<IServiceBusHostConfigurator> configure = null)
        {
            // in case they pass a URI by mistake (it happens)
            if (Uri.IsWellFormedUriString(connectionString, UriKind.Absolute))
            {
                var hostAddress = new Uri(connectionString);

                return Host(configurator, hostAddress, configure);
            }

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            var hostConfigurator = new ServiceBusHostConfigurator(namespaceManager.Address) {TokenProvider = namespaceManager.Settings.TokenProvider};

            if (namespaceManager.Settings.OperationTimeout > TimeSpan.Zero)
                hostConfigurator.OperationTimeout = namespaceManager.Settings.OperationTimeout;

            configure?.Invoke(hostConfigurator);

            return configurator.Host(hostConfigurator.Settings);
        }

        public static void SharedAccessSignature(this IServiceBusHostConfigurator configurator,
            Action<ISharedAccessSignatureTokenProviderConfigurator> configure)
        {
            var tokenProviderConfigurator = new SharedAccessSignatureTokenProviderConfigurator();

            configure(tokenProviderConfigurator);

            configurator.TokenProvider = tokenProviderConfigurator.GetTokenProvider();
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IServiceBusBusFactoryConfigurator configurator, Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(new TemporaryEndpointDefinition(), DefaultEndpointNameFormatter.Instance, configure);
        }

        /// <summary>
        /// Declare a ReceiveEndpoint using a unique generated queue name. This queue defaults to auto-delete
        /// and non-durable. By default all services bus instances include a default receiveEndpoint that is
        /// of this type (created automatically upon the first receiver binding).
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IServiceBusBusFactoryConfigurator configurator, IEndpointDefinition definition,
            Action<IServiceBusReceiveEndpointConfigurator> configure = null)
        {
            configurator.ReceiveEndpoint(definition, DefaultEndpointNameFormatter.Instance, configure);
        }
    }
}
