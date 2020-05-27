namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Monitoring.Health;


    public class ServiceCollectionBusAttachmentConfigurator :
        RegistrationConfigurator,
        IServiceCollectionBusAttachmentConfigurator
    {
        public ServiceCollectionBusAttachmentConfigurator(IServiceCollection collection)
            : base(new DependencyInjectionContainerRegistrar(collection))
        {
            Collection = collection;
        }

        public IServiceCollection Collection { get; }

        public virtual void SetBusAttachmentFactory<T>(T busAttachmentFactory)
            where T : IBusAttachmentRegistrationFactory<IServiceProvider>
        {
            if (busAttachmentFactory == null)
                throw new ArgumentNullException(nameof(busAttachmentFactory));

            ThrowIfAlreadyConfigured(nameof(SetBusAttachmentFactory));

            Collection.AddSingleton(provider => Bind<IBus>.Create(busAttachmentFactory.CreateBusAttachment(GetRegistrationContext(provider))));
        }

        IBusAttachmentRegistrationContext<IServiceProvider> GetRegistrationContext(IServiceProvider provider)
        {
            return new BusAttachmentRegistrationContext<IServiceProvider>(CreateRegistration(provider.GetRequiredService<IConfigurationServiceProvider>()),
                provider.GetRequiredService<BusHealth>(),
                provider);
        }
    }
}
