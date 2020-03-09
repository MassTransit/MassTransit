namespace MassTransit.Pipeline.ConsumerFactories
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;


    public class DelegateConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly Func<TConsumer> _factoryMethod;

        public DelegateConsumerFactory(Func<TConsumer> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        public async Task Send<TMessage>(ConsumeContext<TMessage> context, IPipe<ConsumerConsumeContext<TConsumer, TMessage>> next)
            where TMessage : class
        {
            TConsumer consumer = null;
            try
            {
                consumer = _factoryMethod();
                if (consumer == null)
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

                await next.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, consumer)).ConfigureAwait(false);
            }
            finally
            {
                var disposable = consumer as IDisposable;
                disposable?.Dispose();
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("delegate");
        }
    }
}
