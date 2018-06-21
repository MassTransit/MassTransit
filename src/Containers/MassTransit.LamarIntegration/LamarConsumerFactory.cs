namespace MassTransit.LamarIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Lamar;
    using Scoping;


    public class LamarConsumerFactory<TConsumer> : IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _factory;

        public LamarConsumerFactory(IContainer container)
        {
            _factory = new ScopeConsumerFactory<TConsumer>(new LamarConsumerScopeProvider(container));
        }

        Task IConsumerFactory<TConsumer>.Send<T>(ConsumeContext<T> context, IPipe<ConsumerConsumeContext<TConsumer, T>> next)
        {
            return _factory.Send(context, next);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _factory.Probe(context);
        }
    }
}
