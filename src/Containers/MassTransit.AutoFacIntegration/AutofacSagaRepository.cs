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
namespace MassTransit.AutofacIntegration
{
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;
    using Saga;


    public class AutofacSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly string _name;
        readonly ISagaRepository<TSaga> _repository;
        readonly ILifetimeScope _scope;

        public AutofacSagaRepository(ISagaRepository<TSaga> repository, ILifetimeScope scope, string name)
        {
            _repository = repository;
            _scope = scope;
            _name = name;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("autofac");
            scope.Add("name", _name);

            _repository.Probe(scope);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var lifetimeScope = _scope.BeginLifetimeScope(_name, x => ConfigureScope(x, context)))
            {
                ConsumeContext<T> proxy = context.CreateScope(lifetimeScope);

                await _repository.Send(proxy, policy, next).ConfigureAwait(false);
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (var lifetimeScope = _scope.BeginLifetimeScope(_name))
            {
                SagaQueryConsumeContext<TSaga, T> proxy = context.CreateQueryScope(lifetimeScope);

                await _repository.SendQuery(proxy, policy, next).ConfigureAwait(false);
            }
        }

        protected virtual void ConfigureScope(ContainerBuilder containerBuilder, ConsumeContext context)
        {
            containerBuilder.RegisterInstance(context)
                .ExternallyOwned();
        }
    }
}