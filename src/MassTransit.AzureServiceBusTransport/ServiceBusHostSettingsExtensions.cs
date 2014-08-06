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
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;


    public static class ServiceBusHostSettingsExtensions
    {
        /// <summary>
        /// Create a MessagingFactory from the settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static MessagingFactory GetMessagingFactory(this ServiceBusHostSettings settings)
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = settings.OperationTimeout,
                TransportType = TransportType.Amqp,
                AmqpTransportSettings = new AmqpTransportSettings
                {
                    BatchFlushInterval = TimeSpan.FromMilliseconds(50),
                },
            };

            return MessagingFactory.Create(settings.ServiceUri, mfs);
        }

        public static NamespaceManager GetNamespaceManager(this ServiceBusHostSettings settings)
        {
            var nms = new NamespaceManagerSettings
            {
                TokenProvider = settings.TokenProvider,
                OperationTimeout = TimeSpan.FromSeconds(10),
                RetryPolicy = RetryPolicy.NoRetry,
            };

            return new NamespaceManager(settings.ServiceUri, nms);
        }
    }
}