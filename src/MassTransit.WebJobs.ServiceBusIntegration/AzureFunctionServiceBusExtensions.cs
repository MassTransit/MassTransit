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
namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using AzureServiceBusTransport.Transport;
    using Configuration;
    using Microsoft.Azure.WebJobs;


    public static class AzureFunctionServiceBusExtensions
    {
        public static IBrokeredMessageReceiver CreateBrokeredMessageReceiver(this IBusFactorySelector selector, IBinder binder,
            Action<IWebJobReceiverConfigurator> configure)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);
            var busEndpointConfiguration = busConfiguration.CreateReceiveEndpointConfiguration(new Uri("sb://localhost/"), new Uri("sb://localhost/"));

            var configurator = new WebJobBrokeredMessageReceiverSpecification(binder, busEndpointConfiguration);

            configure(configurator);

            return configurator.Build();
        }

        public static IEventDataReceiver CreateEventDataReceiver(this IBusFactorySelector selector, IBinder binder,
            Action<IWebJobReceiverConfigurator> configure)
        {
            if (binder == null)
                throw new ArgumentNullException(nameof(binder));

            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var endpointConfiguration = new ServiceBusEndpointConfiguration(new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology));

            var topologyConfiguration = new ServiceBusTopologyConfiguration(AzureBusFactory.MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);
            var busEndpointConfiguration = busConfiguration.CreateReceiveEndpointConfiguration(new Uri("sb://localhost/"), new Uri("sb://localhost/"));

            var configurator = new WebJobEventDataReceiverSpecification(binder, busEndpointConfiguration);

            configure(configurator);

            return configurator.Build();
        }
    }
}