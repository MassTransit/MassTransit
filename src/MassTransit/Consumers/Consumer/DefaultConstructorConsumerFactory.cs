namespace MassTransit.Consumer
{
    using System;
    using System.Threading.Tasks;
    using Context;


    public class DefaultConstructorConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class, new()
    {
        public async Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
            where T : class
        {
            TConsumer consumer = null;
            try
            {
                consumer = new TConsumer();

                await next.Send(new ConsumerConsumeContextScope<TConsumer, T>(context, consumer)).ConfigureAwait(false);
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
            context.CreateConsumerFactoryScope<TConsumer>("defaultConstructor");
        }
    }
}
