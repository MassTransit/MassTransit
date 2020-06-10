namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;


    public class ServiceCollectionRiderConfigurator<TBus> :
        ServiceCollectionRiderConfigurator,
        IServiceCollectionRiderConfigurator<TBus>
        where TBus : class, IBus
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(collection, registrar, riderTypes)
        {
        }

        public override void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>());
                var busHealth = provider.GetRequiredService<Bind<TBus, BusHealth>>();
                return new RiderRegistrationContext(registration, busHealth.Value);
            }

            Collection.AddSingleton(provider => Bind<TBus, TRider>.Create(CreateRegistrationContext(provider)));
            Collection.AddSingleton(provider =>
                Bind<TBus>.Create(riderFactory.CreateRider(provider.GetRequiredService<Bind<TBus, TRider, IRiderRegistrationContext>>().Value)));
            Collection.AddSingleton(provider => Bind<TBus>.Create(provider.GetRequiredService<IBusInstance<TBus>>().GetRider<TRider>()));
        }
    }
}
