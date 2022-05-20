namespace MassTransit.Configuration
{
    using System;
    using System.Linq;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Transports;


    public class ServiceCollectionRiderConfigurator :
        RegistrationConfigurator,
        IRiderRegistrationConfigurator
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(collection, registrar)
        {
        }

        public virtual void TryAddScoped<TRider, TService>(Func<TRider, IServiceProvider, TService> factory)
            where TRider : class, IRider
            where TService : class
        {
            this.TryAddScoped(provider => factory(provider.GetRequiredService<Bind<IBus, TRider>>().Value, provider));
        }

        public virtual void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
            where TRider : class, IRider
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider);
                return new RiderRegistrationContext(registration, Registrar);
            }

            this.AddSingleton(provider => Bind<IBus, TRider>.Create(CreateRegistrationContext(provider)));
            this.AddSingleton(provider =>
                Bind<IBus>.Create(riderFactory.CreateRider(provider.GetRequiredService<Bind<IBus, TRider, IRiderRegistrationContext>>().Value)));
            this.AddSingleton(provider => Bind<IBus>.Create(provider.GetRequiredService<Bind<IBus, IBusInstance>>().Value.GetRider<TRider>()));
            this.AddSingleton(provider => provider.GetRequiredService<Bind<IBus, TRider>>().Value);
        }

        protected void ThrowIfAlreadyConfigured<TRider>()
            where TRider : class, IRider
        {
            ThrowIfAlreadyConfigured(nameof(SetRiderFactory));
            var riderType = typeof(TRider);

            if (this.Any(d => d.ServiceType == riderType))
                throw new ConfigurationException($"'{riderType.Name}' has been already registered.");
        }
    }


    public class ServiceCollectionRiderConfigurator<TBus> :
        ServiceCollectionRiderConfigurator,
        IRiderRegistrationConfigurator<TBus>
        where TBus : class, IBus
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(collection, registrar)
        {
        }

        public override void TryAddScoped<TRider, TService>(Func<TRider, IServiceProvider, TService> factory)
        {
            this.TryAddScoped(provider => factory(provider.GetRequiredService<Bind<TBus, TRider>>().Value, provider));
        }

        public override void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider);
                return new RiderRegistrationContext(registration, Registrar);
            }

            this.AddSingleton(provider => Bind<TBus, TRider>.Create(CreateRegistrationContext(provider)));
            this.AddSingleton(provider =>
                Bind<TBus>.Create(riderFactory.CreateRider(provider.GetRequiredService<Bind<TBus, TRider, IRiderRegistrationContext>>().Value)));
            this.AddSingleton(provider => Bind<TBus>.Create(provider.GetRequiredService<IBusInstance<TBus>>().GetRider<TRider>()));
        }
    }
}
