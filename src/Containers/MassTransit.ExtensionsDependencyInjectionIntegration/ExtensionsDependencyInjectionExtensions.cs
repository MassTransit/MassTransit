// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;


    public static class ExtensionsDependencyInjectionIntegrationExtensions
    {
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceProvider serviceProvider)
        {
            var consumerCache = serviceProvider.GetService<IConsumerCacheService>();

            var consumers = consumerCache.GetConfigurators();

            foreach (var consumer in consumers)
                consumer.Configure(configurator, serviceProvider);

            var sagaCache = serviceProvider.GetService<ISagaCacheService>();

            IEnumerable<ICachedConfigurator> sagas = sagaCache.GetConfigurators();

            foreach (var saga in sagas)
                saga.Configure(configurator, serviceProvider);
        }
    }
}