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
namespace MassTransit.AutofacIntegration
{
    using Autofac;


    public class RegistryLifetimeScopeProvider<TId> :
        ILifetimeScopeProvider
    {
        readonly ILifetimeScopeRegistry<TId> _registry;

        public RegistryLifetimeScopeProvider(ILifetimeScopeRegistry<TId> registry)
        {
            _registry = registry;
        }

        public ILifetimeScope LifetimeScope => _registry.GetLifetimeScope(default(TId));

        public ILifetimeScope GetLifetimeScope(ConsumeContext context)
        {
            return _registry.GetLifetimeScope(default(TId));
        }

        ILifetimeScope ILifetimeScopeProvider.GetLifetimeScope<T>(ConsumeContext<T> context)
        {
            var scopeId = GetScopeId(context);

            return _registry.GetLifetimeScope(scopeId);
        }

        TId GetScopeId<T>(ConsumeContext<T> context)
            where T : class
        {
            var scopeId = default(TId);

            // first, try to use the message-based scopeId provider
            if (_registry.TryResolve(out ILifetimeScopeIdAccessor<T, TId> provider) && provider.TryGetScopeId(context.Message, out scopeId))
                return scopeId;

            // second, try to use the consume context based message version
            var idProvider = _registry.ResolveOptional<ILifetimeScopeIdProvider<TId>>(TypedParameter.From(context), TypedParameter.From<ConsumeContext>(context));
            if (idProvider != null && idProvider.TryGetScopeId(out scopeId))
                return scopeId;

            // okay, give up, default it is
            return scopeId;
        }
    }
}