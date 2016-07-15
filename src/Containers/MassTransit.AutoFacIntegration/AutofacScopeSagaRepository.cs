// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Pipeline;
    using Saga;
    using Util;


    public class AutofacScopeSagaRepository<TSaga, TId> :
        ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly string _name;
        readonly ILifetimeScopeRegistry<TId> _registry;
        readonly ISagaRepository<TSaga> _repository;

        public AutofacScopeSagaRepository(ISagaRepository<TSaga> repository, ILifetimeScopeRegistry<TId> registry, string name)
        {
            _repository = repository;
            _registry = registry;
            _name = name;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("autofac");
            scope.Add("scopeTag", _name);
            scope.Add("scopeType", TypeMetadataCache<TId>.ShortName);

            _repository.Probe(scope);
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            var outerScope = _registry.GetLifetimeScope(GetScopeId(context));

            using (outerScope.BeginLifetimeScope(_name))
            {
                await _repository.Send(context, policy, next).ConfigureAwait(false);
            }
        }

        async Task ISagaRepository<TSaga>.SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            var outerScope = _registry.GetLifetimeScope(GetScopeId(context));

            using (outerScope.BeginLifetimeScope(_name))
            {
                await _repository.SendQuery(context, policy, next).ConfigureAwait(false);
            }
        }

        TId GetScopeId<TMessage>(ConsumeContext<TMessage> context)
            where TMessage : class
        {
            var scopeId = default(TId);

            // first, try to use the message-based scopeId provider
            ILifetimeScopeIdAccessor<TMessage, TId> provider;
            if (_registry.TryResolve(out provider) && provider.TryGetScopeId(context.Message, out scopeId))
            {
                return scopeId;
            }

            // second, try to use the consume context based message version
            var idProvider = _registry.ResolveOptional<ILifetimeScopeIdProvider<TId>>(TypedParameter.From(context), TypedParameter.From<ConsumeContext>(context));
            if (idProvider != null && idProvider.TryGetScopeId(out scopeId))
                return scopeId;

            // okay, give up, default it is
            return scopeId;
        }
    }
}