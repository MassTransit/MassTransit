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

namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;


    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register and hosts the resolved bus with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="createBus">Bus factory that loads consumers and sagas from IServiceProvider</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, Func<IServiceProvider, IBusControl> createBus)
        {
            services.TryAddSingleton(createBus);
            services.TryAddSingleton<IBus>(p => p.GetRequiredService<IBusControl>());
            services.TryAddSingleton<IPublishEndpoint>(p => p.GetRequiredService<IBusControl>());
            services.TryAddSingleton<ISendEndpointProvider>(p => p.GetRequiredService<IBusControl>());

            services.AddSingleton<IHostedService>(p => new MassTransitHostedService(p.GetRequiredService<IBusControl>()));

            return services;
        }

        /// <summary>
        /// Register and hosts the resolved bus with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="createBus">Bus factory that loads consumers and sagas from IServiceProvider</param>
        /// <param name="configure">Use MassTransit DI extensions for IServiceCollection to register consumers and sagas</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, Func<IServiceProvider, IBusControl> createBus,
            Action<IServiceCollectionConfigurator> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            services.AddMassTransit(configure);
            services.AddMassTransit(createBus);
            
            return services;
        }

        /// <summary>
        /// Register and hosts a given bus instance with all required interfaces.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="bus">The bus instance</param>
        /// <returns></returns>
        public static IServiceCollection AddMassTransit(this IServiceCollection services, IBusControl bus)
        {
            services.TryAddSingleton(bus);
            services.TryAddSingleton<IBus>(bus);
            services.TryAddSingleton<IPublishEndpoint>(bus);
            services.TryAddSingleton<ISendEndpointProvider>(bus);

            return services;
        }
    }
}