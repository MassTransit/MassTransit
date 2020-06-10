namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Riders;


    public class ServiceCollectionRiderConfigurator :
        RegistrationConfigurator,
        IServiceCollectionRiderConfigurator
    {
        readonly HashSet<Type> _riderTypes;

        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(registrar)
        {
            _riderTypes = riderTypes;
            Collection = collection;
        }

        public IServiceCollection Collection { get; }

        public virtual void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
            where TRider : class, IRider
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>());
                var busHealth = provider.GetRequiredService<BusHealth>();
                return new RiderRegistrationContext(registration, busHealth);
            }

            Collection.AddSingleton(provider => Bind<IBus, TRider>.Create(CreateRegistrationContext(provider)));
            Collection.AddSingleton(provider =>
                Bind<IBus>.Create(riderFactory.CreateRider(provider.GetRequiredService<Bind<IBus, TRider, IRiderRegistrationContext>>().Value)));
            Collection.AddSingleton(provider => Bind<IBus>.Create(provider.GetRequiredService<IBusInstance>().GetRider<TRider>()));
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, TRider>>().Value);
        }

        protected void ThrowIfAlreadyConfigured<TRider>()
            where TRider : IRider
        {
            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));
            if (!_riderTypes.Add(typeof(TRider)))
                throw new ConfigurationException($"'{typeof(TRider).Name}' can be added only once.");
        }
    }
}
