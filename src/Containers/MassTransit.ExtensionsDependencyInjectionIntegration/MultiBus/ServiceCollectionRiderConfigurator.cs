namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;


    public class ServiceCollectionRiderConfigurator<TBus> :
        ServiceCollectionRiderConfigurator,
        IServiceCollectionRiderConfigurator<TBus>
        where TBus : class, IBus
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection)
            : base(collection)
        {
        }

        public override void SetRiderFactory<T>(T riderFactory)
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));

            Collection.AddSingleton(provider => Bind<TBus>.Create(riderFactory.CreateRider(GetRegistrationContext(provider))));
        }

        IRiderRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new RiderRegistrationContext<IServiceProvider>(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<Bind<TBus, BusHealth>>().Value,
                provider);
        }
    }
}
