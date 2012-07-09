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
    using EndpointConfigurators;
    using Transports;

    public static class MessageTrackerConfigurationExtensions
    {
        public static T SetDefaultRetryLimit<T>(this T configurator, int retryLimit)
            where T : EndpointFactoryConfigurator
        {
            var serializerConfigurator = new DefaultRetryLimitEndpointFactoryConfigurator(retryLimit);

            configurator.AddEndpointFactoryConfigurator(serializerConfigurator);

            return configurator;
        }

        public static T SetDefaultInboundMessageTrackerFactory<T>(this T configurator,
            MessageTrackerFactory messageTrackerFactory)
            where T : EndpointFactoryConfigurator
        {
            var serializerConfigurator = new DefaultMessageTrackerEndpointFactoryConfigurator(messageTrackerFactory);

            configurator.AddEndpointFactoryConfigurator(serializerConfigurator);

            return configurator;
        }
    }
}