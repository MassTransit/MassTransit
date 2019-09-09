namespace MassTransit.AutofacIntegration
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using GreenPipes;
    using ScopeProviders;
    using Scoping;


    public class AutofacConsumerFactory<TConsumer> :
        IConsumerFactory<TConsumer>
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _factory;

        public AutofacConsumerFactory(ILifetimeScope scope, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, configureScope);

            _factory = new ScopeConsumerFactory<TConsumer>(scopeProvider);
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
