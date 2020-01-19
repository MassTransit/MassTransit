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
namespace MassTransit.Scoping
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Saga;


    /// <summary>
    /// A generic scoped saga repository, which can be leveraged by any container
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class ScopeSagaRepository<TSaga> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _repository;
        readonly ISagaScopeProvider<TSaga> _scopeProvider;

        public ScopeSagaRepository(ISagaRepository<TSaga> repository, ISagaScopeProvider<TSaga> scopeProvider)
        {
            _repository = repository;
            _scopeProvider = scopeProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("scope");
            _scopeProvider.Probe(scope);

            _repository.Probe(scope);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (ISagaScopeContext<T> scope = _scopeProvider.GetScope(context))
            {
                await _repository.Send(scope.Context, policy, next).ConfigureAwait(false);
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            using (ISagaScopeContext<T> scope = _scopeProvider.GetScope(context))
            {
                await _repository.SendQuery(scope.Context, query, policy, next).ConfigureAwait(false);
            }
        }
    }
}
