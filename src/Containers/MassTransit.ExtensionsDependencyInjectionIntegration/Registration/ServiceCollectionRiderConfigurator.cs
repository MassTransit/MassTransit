namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Riders;


    public class ServiceCollectionRiderConfigurator :
        RegistrationConfigurator,
        IServiceCollectionRiderConfigurator
    {
        readonly HashSet<Type> _riderTypes;
        protected readonly RegistrationCache<object> Registrations;

        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar, HashSet<Type> riderTypes)
            : base(registrar)
        {
            _riderTypes = riderTypes;
            Collection = collection;
            Registrations = new RegistrationCache<object>();
        }

        public IServiceCollection Collection { get; }

        public void AddRegistration<T>(T registration)
            where T : class
        {
            Registrations.GetOrAdd(typeof(T), _ => registration);
        }

        public virtual void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
            where TRider : class, IRider
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>());
                return new RiderRegistrationContext(registration, Registrations);
            }

            Collection.AddSingleton(provider => Bind<IBus, TRider>.Create(CreateRegistrationContext(provider)));
            Collection.AddSingleton(provider =>
                Bind<IBus>.Create(riderFactory.CreateRider(provider.GetRequiredService<Bind<IBus, TRider, IRiderRegistrationContext>>().Value)));
            Collection.AddSingleton(provider => Bind<IBus>.Create(provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.GetRider<TRider>()));
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, TRider>>().Value);
        }

        protected void ThrowIfAlreadyConfigured<TRider>()
            where TRider : IRider
        {
            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));
            var riderType = typeof(TRider);
            if (!_riderTypes.Add(riderType))
                throw new ConfigurationException($"'{riderType.Name}' can be added only once.");

            //TODO: maybe support it at some point....
            if (Collection.Any(d => d.ServiceType == riderType))
                throw new ConfigurationException($"'{riderType.Name}' has been already registered.");
        }
    }
}
