namespace MassTransit.NinjectIntegration
{
    using Ninject;
    using Ninject.Extensions.NamedScope;
    using Saga;


    public class KernelConfigurator :
        IKernelConfigurator
    {
        readonly IConsumerRegistry _consumerRegistry;
        readonly IKernel _kernel;
        readonly ISagaRegistry _sagaRegistry;

        public KernelConfigurator(IKernel kernel)
        {
            _kernel = kernel;

            var namedScopeModule = new NamedScopeModule();
            if (!kernel.HasModule(namedScopeModule.Name))
                kernel.Load(namedScopeModule);

            _consumerRegistry = kernel.TryGet<IConsumerRegistry>();
            if (_consumerRegistry == null)
            {
                kernel.Bind<IConsumerRegistry>().To<ConsumerRegistry>().InSingletonScope();
                _consumerRegistry = kernel.Get<IConsumerRegistry>();
            }

            _sagaRegistry = kernel.TryGet<ISagaRegistry>();
            if (_sagaRegistry == null)
            {
                kernel.Bind<ISagaRegistry>().To<SagaRegistry>().InSingletonScope();
                _sagaRegistry = kernel.Get<ISagaRegistry>();
            }
        }

        public void AddConsumer<T>()
            where T : class, IConsumer
        {
            _kernel.Bind<T>().ToSelf().DefinesNamedScope("message");

            _consumerRegistry.Add<T>();
        }

        public void AddSaga<T>()
            where T : class, ISaga
        {
            _sagaRegistry.Add<T>();
        }
    }
}
