namespace MassTransit.LamarIntegration
{
    using Lamar;


    public interface IServiceRegistryMediatorConfigurator :
        IMediatorRegistrationConfigurator<IServiceContext>
    {
        ServiceRegistry Builder { get; }
    }
}
