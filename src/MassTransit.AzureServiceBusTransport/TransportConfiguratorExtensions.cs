// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Configuration;


    public static class TransportConfiguratorExtensions
    {
        public static ServiceBusHostSettings Host(this IServiceBusBusFactoryConfigurator configurator, Uri hostAddress,
            Action<IServiceBusHostConfigurator> configure)
        {
            var hostConfigurator = new AzureServiceBusHostConfigurator(hostAddress);

            configure(hostConfigurator);

            configurator.Host(hostConfigurator.Settings);

            return hostConfigurator.Settings;
        }

        public static void SharedAccessSignature(this IServiceBusHostConfigurator configurator,
            Action<ISharedAccessSignatureTokenProviderConfigurator> configure)
        {
            var tokenProviderConfigurator = new SharedAccessSignatureTokenProviderConfigurator();

            configure(tokenProviderConfigurator);

            configurator.TokenProvider = tokenProviderConfigurator.GetTokenProvider();
        }


        /// <summary>
        /// Declare a ReceiveEndpoint on the broker and configure the endpoint settings and message consumers.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="hostSettings">The host for this endpoint</param>
        /// <param name="queueName">The input queue name</param>
        /// <param name="configure">The configuration method</param>
        public static void ReceiveEndpoint(this IServiceBusBusFactoryConfigurator configurator, ServiceBusHostSettings hostSettings, string queueName,
            Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            var endpointConfigurator = new ServiceBusReceiveEndpointConfigurator(hostSettings, queueName);

            configure(endpointConfigurator);

            configurator.AddServiceBusFactoryBuilderConfigurator(endpointConfigurator);
        }

    }
}