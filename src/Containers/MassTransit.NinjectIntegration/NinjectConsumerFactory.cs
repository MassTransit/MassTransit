namespace MassTransit.NinjectIntegration
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Metadata;
    using Ninject;


    public class NinjectConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IKernel _kernel;

        public NinjectConsumerFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
            where T : class
        {
            var consumer = _kernel.Get<TConsumer>();
            if (consumer == null)
                throw new ConsumerException($"Unable to resolve consumer type '{TypeMetadataCache<TConsumer>.ShortName}'.");

            try
            {
                await next.Send(new ConsumerConsumeContextScope<TConsumer, T>(context, consumer)).ConfigureAwait(false);
            }
            finally
            {
                _kernel.Release(consumer);
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("ninject");
        }
    }
}
