namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionBusAttachmentConfigurator :
        IBusAttachmentRegistrationConfigurator<IServiceProvider>
    {
        IServiceCollection Collection { get; }
    }
}
