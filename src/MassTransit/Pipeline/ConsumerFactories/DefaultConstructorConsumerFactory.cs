namespace MassTransit.Pipeline.ConsumerFactories
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


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
                var disposable = consumer as IDisposable;
                disposable?.Dispose();
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("defaultConstructor");
        }
    }
}
