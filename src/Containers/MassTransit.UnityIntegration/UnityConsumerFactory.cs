namespace MassTransit.UnityIntegration
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Scoping;
    using Unity;


    public class UnityConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _factory;

        public UnityConsumerFactory(IUnityContainer container)
        {
            _factory = new ScopeConsumerFactory<TConsumer>(new UnityConsumerScopeProvider(container));
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
