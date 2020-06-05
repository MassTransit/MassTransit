namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Riders;


    public class ServiceCollectionRiderConfigurator :
        RegistrationConfigurator,
        IServiceCollectionRiderConfigurator
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(registrar)
        {
            Collection = collection;
        }

        public IServiceCollection Collection { get; }

        public virtual void SetRiderFactory<TRider>(IRegistrationRiderFactory<IServiceProvider, TRider> riderFactory)
            where TRider : class, IRider
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));

            Collection.AddSingleton(provider => Bind<IBus>.Create(riderFactory.CreateRider(GetRegistrationContext(provider))));
            Collection.AddSingleton(provider => Bind<IBus>.Create(provider.GetRequiredService<IBusInstance>().GetRider<TRider>()));
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, TRider>>().Value);
        }

        IRiderRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new RiderRegistrationContext<IServiceProvider>(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<BusHealth>(),
                provider);
        }
    }
}
