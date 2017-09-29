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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public class ServiceCollectionConfigurator :
        IServiceCollectionConfigurator
    {
        readonly IConsumerCacheService _consumerCacheService;
        readonly ISagaCacheService _sagaCacheService;
        readonly IServiceCollection _services;

        public ServiceCollectionConfigurator(IServiceCollection services)
        {
            _services = services;

            _consumerCacheService = new ConsumerCacheService();
            _sagaCacheService = new SagaCacheService();

            services.AddSingleton(_consumerCacheService);
            services.AddSingleton(_sagaCacheService);
        }

        public void AddConsumer<T>()
            where T : class, IConsumer
        {
            _services.AddScoped<T>();

            _consumerCacheService.Add<T>();
        }

        public void AddSaga<T>()
            where T : class, ISaga
        {
            _sagaCacheService.Add<T>();
        }
    }
}