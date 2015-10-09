namespace MassTransit.SimpleInjectorIntegration
{
    using System.Threading.Tasks;

    using SimpleInjector;

    using MassTransit.Pipeline;
    using MassTransit.Util;

    using SimpleInjector.Extensions.ExecutionContextScoping;


    public class SimpleInjectorConsumerFactory<TConsumer> :
            IConsumerFactory<TConsumer>
            where TConsumer : class
    {
        readonly Container _container;

        public SimpleInjectorConsumerFactory(Container container)
        {
            this._container = container;
        }

        async Task IConsumerFactory<TConsumer>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
        {
            using (var scope = this._container.BeginExecutionContextScope())
            {
                var consumer = this._container.GetInstance<TConsumer>();
                if (consumer == null)
                {
                    throw new ConsumerException(
                        string.Format("Unable to resolve consumer type '{0}'.", TypeMetadataCache<TConsumer>.ShortName));
                }

                await next.Send(context.PushConsumer(consumer));
            }
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateConsumerFactoryScope<TConsumer>("simpleinjector");
        }
    }
}
