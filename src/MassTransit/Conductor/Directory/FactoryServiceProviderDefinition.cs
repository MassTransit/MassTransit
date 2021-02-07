namespace MassTransit.Conductor.Directory
{
    using System.Threading.Tasks;


    public class FactoryServiceProviderDefinition<TInput, TResult> :
        IServiceProviderDefinition<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        readonly AsyncMessageFactory<TInput, TResult> _factory;

        public FactoryServiceProviderDefinition(MessageFactory<TInput, TResult> factory)
        {
            _factory = context => Task.FromResult(factory(context));
        }

        public FactoryServiceProviderDefinition(AsyncMessageFactory<TInput, TResult> factory)
        {
            _factory = factory;
        }

        public void Configure(IServiceRegistrationConfigurator<TInput> configurator)
        {
        }

        public IProviderRegistration<TInput, TResult> CreateProvider()
        {
            return new FactoryProviderRegistration<TInput, TResult>(_factory);
        }
    }
}
