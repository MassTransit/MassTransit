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
namespace MassTransit.StructureMapIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;
    using StructureMap;


    public class StructureMapSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IContainer _container;
        readonly ISagaRepository<TSaga> _repository;

        public StructureMapSagaRepository(ISagaRepository<TSaga> repository, IContainer container)
        {
            _repository = repository;
            _container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("structuremap");

            _repository.Probe(scope);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                ConsumeContext<T> proxy = context.CreateScope(nestedContainer);

                nestedContainer.Configure(x =>
                {
                    x.For<ConsumeContext>()
                        .Use(proxy);
                    x.For<ConsumeContext<T>>()
                        .Use(proxy);
                });

                await _repository.Send(proxy, policy, next).ConfigureAwait(false);
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                SagaQueryConsumeContext<TSaga, T> proxy = context.CreateQueryScope(nestedContainer);

                await _repository.SendQuery(proxy, policy, next).ConfigureAwait(false);
            }
        }
    }
}