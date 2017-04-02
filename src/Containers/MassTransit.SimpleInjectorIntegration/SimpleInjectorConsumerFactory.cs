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
namespace MassTransit.SimpleInjectorIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Util;


    public class SimpleInjectorConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly Container _container;

        public SimpleInjectorConsumerFactory(Container container)
        {
            _container = container;
        }

        async Task IConsumerFactory<TConsumer>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
        {
            using (var scope = AsyncScopedLifestyle.BeginScope(_container))
            {
                var consumer = _container.GetInstance<TConsumer>();
                if (consumer == null)
                {
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");
                }

                ConsumerConsumeContext<TConsumer, T> consumerConsumeContext = context.PushConsumerScope(consumer, scope);

                await next.Send(consumerConsumeContext).ConfigureAwait(false);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("simpleinjector");
        }
    }
}