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
    using Pipeline;
    using Util;


    public class AutofacConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly string _name;
        readonly ILifetimeScope _scope;

        public AutofacConsumerFactory(ILifetimeScope scope, string name)
        {
            _scope = scope;
            _name = name;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            using (var innerScope = _scope.BeginLifetimeScope(_name, x => ConfigureScope(x, context)))
            {
                var consumer = innerScope.Resolve<TConsumer>();
                if (consumer == null)
                {
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");
                }

                ConsumerConsumeContext<TConsumer, TMessage> consumerConsumeContext = context.PushConsumerScope(consumer, innerScope);

                await next.Send(consumerConsumeContext).ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateConsumerFactoryScope<TConsumer>("autofac");
            scope.Add("scopeTag", _name);
        }

        static void ConfigureScope(ContainerBuilder containerBuilder, ConsumeContext context)
        {
            containerBuilder.RegisterInstance(context)
                .ExternallyOwned();
        }
    }
}