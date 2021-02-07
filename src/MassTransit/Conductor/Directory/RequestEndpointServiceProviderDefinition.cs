namespace MassTransit.Conductor.Directory
{
    public class RequestEndpointServiceProviderDefinition<TInput, TResult> :
        IServiceProviderDefinition<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        public void Configure(IServiceRegistrationConfigurator<TInput> configurator)
        {
        }

        public IProviderRegistration<TInput, TResult> CreateProvider()
        {
            return new RequestEndpointProviderRegistration<TInput, TResult>();
        }
    }
}
