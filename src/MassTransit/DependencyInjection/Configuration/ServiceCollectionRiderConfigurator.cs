namespace MassTransit.Configuration
{
    using System;
    using System.Linq;
    using DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


    public class ServiceCollectionRiderConfigurator :
        RegistrationConfigurator,
        IRiderRegistrationConfigurator
    {
        public ServiceCollectionRiderConfigurator(IServiceCollection collection, IContainerRegistrar registrar)
            : base(collection, registrar)
        {
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
                return new RiderRegistrationContext(registration);
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

        public override void SetRiderFactory<TRider>(IRegistrationRiderFactory<TRider> riderFactory)
        {
            if (riderFactory == null)
                throw new ArgumentNullException(nameof(riderFactory));

            ThrowIfAlreadyConfigured<TRider>();

            IRiderRegistrationContext CreateRegistrationContext(IServiceProvider provider)
            {
                var registration = CreateRegistration(provider);
                return new RiderRegistrationContext(registration);
            }

            this.AddSingleton(provider => Bind<TBus, TRider>.Create(CreateRegistrationContext(provider)));
            this.AddSingleton(provider =>
                Bind<TBus>.Create(riderFactory.CreateRider(provider.GetRequiredService<Bind<TBus, TRider, IRiderRegistrationContext>>().Value)));
            this.AddSingleton(provider => Bind<TBus>.Create(provider.GetRequiredService<IBusInstance<TBus>>().GetRider<TRider>()));
        }
    }
}
