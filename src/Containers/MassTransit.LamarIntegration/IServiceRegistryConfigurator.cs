namespace MassTransit.LamarIntegration
{
    using Lamar;


    public interface IServiceRegistryConfigurator :
        IRegistrationConfigurator<IServiceContext>
    {
        ServiceRegistry Builder { get; }
    }
}
