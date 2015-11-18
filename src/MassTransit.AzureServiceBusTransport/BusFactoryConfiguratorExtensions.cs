// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Util;


    public static class BusFactoryConfiguratorExtensions
    {
        public static IServiceBusHost Host(this IServiceBusBusFactoryConfigurator configurator, Uri hostAddress,
            Action<IServiceBusHostConfigurator> configure)
        {
            var hostConfigurator = new AzureServiceBusHostConfigurator(hostAddress);

            configure(hostConfigurator);

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
        /// <param name="host"></param>
        /// <param name="configure"></param>
        public static void ReceiveEndpoint(this IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host,
            Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            var queueName = configurator.GetTemporaryQueueName("endpoint");

            configurator.ReceiveEndpoint(host, queueName, x =>
            {
                x.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                x.EnableExpress = true;

                configure(x);
            });
        }

    }
}