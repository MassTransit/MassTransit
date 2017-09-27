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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Util;


    public class ExtensionsDependencyInjectionConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IServiceProvider _services;

        public ExtensionsDependencyInjectionConsumerFactory(IServiceProvider services)
        {
            _services = services;
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next) where T : class
        {
            var scopeFactory = _services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetService<TConsumer>();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                ConsumerConsumeContext<TConsumer, T> consumerConsumeContext = context.PushConsumerScope(consumer, scope);

                await next.Send(consumerConsumeContext).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("msedi");
        }
    }
}