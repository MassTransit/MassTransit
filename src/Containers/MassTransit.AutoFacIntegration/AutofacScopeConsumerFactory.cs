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
    using Util;


    /// <summary>
    /// Uses a lifetime scope registry to maintain separate containers based on a scopeId that is extracted from the message
    /// </summary>
    /// <typeparam name="TConsumer"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class AutofacScopeConsumerFactory<TConsumer, TId> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly string _name;
        readonly ILifetimeScopeRegistry<TId> _registry;

        public AutofacScopeConsumerFactory(ILifetimeScopeRegistry<TId> registry, string name)
        {
            _registry = registry;
            _name = name;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            var scopeId = GetScopeId(context);

            var outerScope = _registry.GetLifetimeScope(scopeId);

            using (var innerScope = outerScope.BeginLifetimeScope(_name))
            {
                var consumer = innerScope.Resolve<TConsumer>();
                if (consumer == null)
                {
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");
                }

                await next.Send(context.PushConsumer(consumer));
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<TConsumer>("autofac");
            scope.Add("scopeTag", _name);
            scope.Add("scopeType", TypeMetadataCache<TId>.ShortName);
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