namespace MassTransit.Conductor.Directory
{
    using System.Threading.Tasks;
    using Initializers;


    public class InitializerServiceProviderDefinition<TInput, TResult> :
        IServiceProviderDefinition<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        readonly ResultValueProvider<TInput> _valueProvider;

        public InitializerServiceProviderDefinition(ResultValueProvider<TInput> valueProvider)
        {
            _valueProvider = valueProvider;
        }

        public void Configure(IServiceRegistrationConfigurator<TInput> configurator)
        {
        }

        public IProviderRegistration<TInput, TResult> CreateProvider()
        {
            Task<TResult> Factory(OrchestrationContext<TInput> context)
            {
                var values = _valueProvider(context);

                return MessageInitializerCache<TResult>.InitializeMessage(context, values);
            }

            return new FactoryProviderRegistration<TInput, TResult>(Factory);
        }
    }
}
