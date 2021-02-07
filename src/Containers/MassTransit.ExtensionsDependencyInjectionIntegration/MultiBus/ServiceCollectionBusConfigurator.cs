namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Definition;
    using MassTransit.MultiBus;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;
    using Scoping;
    using Transports;


    public class ServiceCollectionBusConfigurator<TBus, TBusInstance> :
        ServiceCollectionBusConfigurator,
        IServiceCollectionConfigurator<TBus>
        where TBus : class, IBus
        where TBusInstance : BusInstance<TBus>, TBus
    {
        public ServiceCollectionBusConfigurator(IServiceCollection collection)
            : base(collection, new DependencyInjectionContainerRegistrar<TBus>(collection))
        {
            IBusRegistrationContext CreateRegistrationContext(IServiceProvider serviceProvider)
            {
                var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();
                var busHealth = serviceProvider.GetRequiredService<Bind<TBus, BusHealth>>();
                return new BusRegistrationContext(provider, busHealth.Value, Endpoints, Consumers, Sagas, ExecuteActivities, Activities, Futures);
            }

            collection.AddScoped(provider => Bind<TBus>.Create(GetSendEndpointProvider(provider)));
            collection.AddScoped(provider => Bind<TBus>.Create(GetPublishEndpoint(provider)));
            collection.AddSingleton(provider =>
                Bind<TBus>.Create(ClientFactoryProvider(provider.GetRequiredService<IConfigurationServiceProvider>(), provider.GetRequiredService<TBus>())));

            collection.AddSingleton(provider => Bind<TBus>.Create(new BusHealth(FormatBusHealthName())));
            collection.AddSingleton<IBusHealth>(provider => provider.GetRequiredService<Bind<TBus, BusHealth>>().Value);

            collection.AddSingleton(provider => Bind<TBus>.Create(CreateRegistrationContext(provider)));
        }

        public override void AddBus(Func<IBusRegistrationContext, IBusControl> busFactory)
        {
            SetBusFactory(new RegistrationBusFactory(busFactory));
        }

        public override void SetBusFactory<T>(T busFactory)
        {
            if (busFactory == null)
                throw new ArgumentNullException(nameof(busFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusFactory));

            Collection.AddSingleton(provider => Bind<TBus>.Create(CreateBus(busFactory, provider)));

            Collection.AddSingleton<IBusInstance>(provider => provider.GetRequiredService<Bind<TBus, IBusInstance<TBus>>>().Value);
            Collection.AddSingleton(provider =>
                Bind<TBus>.Create<IReceiveEndpointConnector>(provider.GetRequiredService<Bind<TBus, IBusInstance<TBus>>>().Value));
            Collection.AddSingleton(provider => provider.GetRequiredService<Bind<TBus, IBusInstance<TBus>>>().Value.BusInstance);
        }

        public override void AddRider(Action<IRiderRegistrationConfigurator> configure)
        {
            var configurator = new ServiceCollectionRiderConfigurator<TBus>(Collection, Registrar, RiderTypes);
            configure?.Invoke(configurator);
        }

        static IBusInstance<TBus> CreateBus<T>(T busFactory, IServiceProvider provider)
            where T : IRegistrationBusFactory
        {
            IEnumerable<IBusInstanceSpecification> specifications = provider.GetServices<Bind<TBus, IBusInstanceSpecification>>().Select(x => x.Value);

            var instance = busFactory.CreateBus(provider.GetRequiredService<Bind<TBus, IBusRegistrationContext>>().Value, specifications);

            var busInstance = provider.GetService<TBusInstance>() ?? ActivatorUtilities.CreateInstance<TBusInstance>(provider, instance.BusControl);

            return new MultiBusInstance<TBus>(busInstance, instance);
        }

        static ISendEndpointProvider GetSendEndpointProvider(IServiceProvider provider)
        {
            return new ScopedSendEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider);
        }

        static IPublishEndpoint GetPublishEndpoint(IServiceProvider provider)
        {
            return new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(provider.GetRequiredService<TBus>(), provider));
        }

        static string FormatBusHealthName()
        {
            string name = typeof(TBus).Name;
            if (name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]))
                name = name.Substring(1);

            return $"masstransit-{KebabCaseEndpointNameFormatter.Instance.SanitizeName(name)}";
        }
    }
}
