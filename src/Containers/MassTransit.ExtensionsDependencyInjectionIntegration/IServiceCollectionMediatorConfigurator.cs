namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionMediatorConfigurator :
        IMediatorRegistrationConfigurator
    {
        IServiceCollection Collection { get; }
    }
}
