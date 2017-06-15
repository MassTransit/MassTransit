using System;
using System.Threading.Tasks;
using MassTransit.Pipeline;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    public class MicrosoftExtensionsDependencyInjectionConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        private readonly IServiceProvider _services;

        public MicrosoftExtensionsDependencyInjectionConsumerFactory(IServiceProvider services)
        {
            _services = services;
        }

        async Task IConsumerFactory<TConsumer>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
        {
            var scopeFactory = _services.GetRequiredService<IServiceScopeFactory>();

            using(var scope = scopeFactory.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetService<TConsumer>();
                if( consumer == null)
                {
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");
                }

                var consumerConsumeContext = context.PushConsumer(consumer);

                await next.Send(consumerConsumeContext).ConfigureAwait(false);
            }            
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("msedi");
        }
    }
}