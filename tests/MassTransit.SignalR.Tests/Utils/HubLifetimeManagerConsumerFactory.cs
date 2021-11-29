namespace MassTransit.SignalR.Tests.Utils
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Metadata;
    using Microsoft.AspNetCore.SignalR;


    public class HubLifetimeManagerConsumerFactory<TConsumer, THub> :
        IConsumerFactory<TConsumer>,
        IHubManagerConsumerFactory<THub>
        where TConsumer : class, IConsumer
        where THub : Hub
    {
        readonly Func<MassTransitHubLifetimeManager<THub>, TConsumer> _factoryMethod;

        public HubLifetimeManagerConsumerFactory(Func<MassTransitHubLifetimeManager<THub>, TConsumer> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            if (HubLifetimeManager == null)
                throw new ArgumentNullException(nameof(HubLifetimeManager));

            TConsumer consumer = null;
            try
            {
                consumer = _factoryMethod(HubLifetimeManager);

                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeCache<TConsumer>.ShortName}'.");

                await next.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, consumer)).ConfigureAwait(false);
            }
            finally
            {
                var disposable = consumer as IDisposable;
                disposable?.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("hubLifetimeManagerFactory");
        }

        public MassTransitHubLifetimeManager<THub> HubLifetimeManager { get; set; }
    }
}
