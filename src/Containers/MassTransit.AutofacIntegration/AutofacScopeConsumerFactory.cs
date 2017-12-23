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
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;
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

        public Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            if (context.TryGetPayload<ILifetimeScope>(out var existingScope))
            {
                ConsumerConsumeContext<TConsumer, TMessage> consumerContext = existingScope.GetConsumer<TConsumer, TMessage>(context);

                return next.Send(consumerContext);
            }

            return SendInScope(context, next);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<TConsumer>("autofac");
            scope.Add("scopeTag", _name);
            scope.Add("scopeType", TypeMetadataCache<TId>.ShortName);
        }

        async Task SendInScope<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next) where TMessage : class
        {
            var scope = _registry.GetLifetimeScope(context);

            using (var consumerScope = scope.BeginLifetimeScope(_name, builder => builder.ConfigureScope(context)))
            {
                ConsumerConsumeContext<TConsumer, TMessage> consumerContext = consumerScope.GetConsumerScope<TConsumer, TMessage>(context);

                await next.Send(consumerContext).ConfigureAwait(false);
            }
        }
    }
}