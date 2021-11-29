namespace MassTransit.Consumer
{
    using System;
    using System.Threading.Tasks;
    using Context;


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
                    throw new ConsumerException($"Unable to resolve consumer type '{TypeCache<TConsumer>.ShortName}'.");

                await next.Send(new ConsumerConsumeContextScope<TConsumer, TMessage>(context, consumer)).ConfigureAwait(false);
            }
            finally
            {
                switch (consumer)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("delegate");
        }
    }
}
