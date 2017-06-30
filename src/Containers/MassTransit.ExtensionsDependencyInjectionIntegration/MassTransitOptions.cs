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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using MassTransit.Util;
    using Microsoft.Extensions.DependencyInjection;
    using System.Collections.Generic;
    using MassTransit.Saga;

    public class MassTransitOptions
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _provider;
        private IConsumerCacheService _consumerCacheService;
        private ISagaCacheService _sagaCacheService;

        public MassTransitOptions(IServiceCollection services)
        {
            _services = services;
            _provider = services.BuildServiceProvider();

            _consumerCacheService = _provider.GetService<IConsumerCacheService>();
            _sagaCacheService = _provider.GetService<ISagaCacheService>();
        }

        public void AddConsumer<T>()
            where T : class, IConsumer
        {
            _services.AddScoped<T>();

            // add to CacheService
            _consumerCacheService.Add<T>();
            
        }

        public void AddSaga<T>()
            where T : class, ISaga
        {
            _services.AddScoped<T>();

            _sagaCacheService.Add<T>();
        }
    }
}