namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;


    public class ServiceCollectionRiderConfigurator :
        RegistrationConfigurator,
        IServiceCollectionRiderConfigurator
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection)
            : base(new DependencyInjectionContainerRegistrar(collection))
        {
            Collection = collection;
        }

        public IServiceCollection Collection { get; }

        public virtual void SetRiderFactory<T>(T riderFactory)
            where T : IRegistrationRiderFactory<IServiceProvider>
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));

            Collection.AddSingleton(provider => Bind<IBus>.Create(riderFactory.CreateRider(GetRegistrationContext(provider))));
        }

        IRiderRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new RiderRegistrationContext<IServiceProvider>(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<BusHealth>(),
                provider);
        }
    }
}
