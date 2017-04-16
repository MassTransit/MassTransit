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
namespace MassTransit.UnityIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Practices.Unity;
    using Saga;


    public class UnitySagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IUnityContainer _container;
        readonly ISagaRepository<TSaga> _repository;

        public UnitySagaRepository(ISagaRepository<TSaga> repository, IUnityContainer container)
        {
            _repository = repository;
            _container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("unity");

            _repository.Probe(scope);
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next) where T : class
        {
            using (var container = _container.CreateChildContainer())
            {
                ConsumeContext<T> proxy = context.CreateScope(container);

                await _repository.Send(proxy, policy, next).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            using (var container = _container.CreateChildContainer())
            {
                SagaQueryConsumeContext<TSaga, T> proxy = context.CreateQueryScope(container);

                await _repository.SendQuery(proxy, policy, next).ConfigureAwait(false);
            }
        }
    }
}