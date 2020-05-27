namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;
    using Registration;


    public class ServiceCollectionBusAttachmentConfigurator<TBus> :
        ServiceCollectionBusAttachmentConfigurator,
        IServiceCollectionBusAttachmentConfigurator<TBus>
        where TBus : class, IBus
    {
        public ServiceCollectionBusAttachmentConfigurator(IServiceCollection collection)
            : base(collection)
        {
        }

        public override void SetBusAttachmentFactory<T>(T busAttachmentFactory)
        {
            if (busAttachmentFactory == null)
                throw new ArgumentNullException(nameof(busAttachmentFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusAttachmentFactory));

            Collection.AddSingleton(provider => Bind<TBus>.Create(busAttachmentFactory.CreateBusAttachment(GetRegistrationContext(provider))));
        }

        IBusAttachmentRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new BusAttachmentRegistrationContext<IServiceProvider>(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<Bind<TBus, BusHealth>>().Value,
                provider);
        }
    }
}
