// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using BusConfigurators;
    using EndpointConfigurators;
    using Transports.Msmq;
    using Transports.Msmq.Configuration;


    public static class MsmqBusConfigurationExtensions
    {
        public static T UseMsmq<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.AddTransportFactory<MsmqTransportFactory>()
                        .AddTransportFactory<MulticastMsmqTransportFactory>();

            return configurator;
        }

        /// <summary>
        /// Use MSMQ, and allow the configuration of additional options, such as Multicast or SubscriptionService usage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ServiceBusConfigurator UseMsmq(this ServiceBusConfigurator configurator,
            Action<MsmqConfigurator> callback)
        {
            configurator.AddTransportFactory<MsmqTransportFactory>()
                        .AddTransportFactory<MulticastMsmqTransportFactory>();

            var msmqConfigurator = new MsmqConfiguratorImpl(configurator);
            callback(msmqConfigurator);

            return configurator;
        }
    }
}