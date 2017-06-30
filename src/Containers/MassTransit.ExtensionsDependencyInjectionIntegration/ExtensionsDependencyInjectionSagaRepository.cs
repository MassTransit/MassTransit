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
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;
    using Microsoft.Extensions.DependencyInjection;

    public class ExtensionsDependencyInjectionSagaRepository<TSaga> : ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        private readonly IServiceProvider _services;
        private readonly ISagaRepository<TSaga> _repository;

        public ExtensionsDependencyInjectionSagaRepository(ISagaRepository<TSaga> repository, IServiceProvider services)
        {
            _services = services;
            _repository = repository;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("msedi");

            _repository.Probe(scope);
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            var scopeFactory = _services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                ConsumeContext<T> proxy = context.CreateScope(scope);

                await _repository.Send(proxy, policy, next).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            var scopeFactory = _services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                SagaQueryConsumeContext<TSaga, T> proxy = context.CreateQueryScope(scope);

                await _repository.SendQuery(proxy, policy, next).ConfigureAwait(false);
            }
        }
    }
}