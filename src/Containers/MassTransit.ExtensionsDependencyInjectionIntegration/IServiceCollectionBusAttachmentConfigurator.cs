namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using MassTransit.Registration.Attachments;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionBusAttachmentConfigurator :
        IBusAttachmentRegistrationConfigurator<IServiceProvider>
    {
        IServiceCollection Collection { get; }
    }
}
