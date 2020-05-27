namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionBusAttachmentConfigurator<in TBus> :
        IBusAttachmentRegistrationConfigurator<IServiceProvider>
        where TBus : class, IBus
    {
        IServiceCollection Collection { get; }
    }
}
