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
    using MassTransit.ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class ExtensionsDependencyInjectionIntegrationExtensions
    {
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceProvider services)
        {
            // get IConsumerCacheService and pull Consumers

            var consumerCache = services.GetService<IConsumerCacheService>();

            var consumers = consumerCache.Instance.Keys;

            foreach (var consumer in consumers)
            {
                consumerCache.Configure(consumer, configurator, services);
            }

            // get ISagaCacheService and pull Sagas

            var sagaCache = services.GetService<ISagaCacheService>();

            var sagas = sagaCache.Instance.Keys;

            foreach (var saga in sagas)
            {
                sagaCache.Configure(saga, configurator, services);
            }

        }
    }
}