using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Pipeline;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    public class ExtensionsDependencyInjectionConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        private readonly IServiceProvider _services;

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
                {
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");
                }

                var consumerConsumeContext = context.PushConsumer(consumer);

                await next.Send(consumerConsumeContext).ConfigureAwait(false);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("msedi");
        }
    }
}