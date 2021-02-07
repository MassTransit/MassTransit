namespace MassTransit.Conductor.Directory
{
    public interface IServiceProviderDefinition<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        void Configure(IServiceRegistrationConfigurator<TInput> configurator);

        IProviderRegistration<TInput, TResult> CreateProvider();
    }
}
