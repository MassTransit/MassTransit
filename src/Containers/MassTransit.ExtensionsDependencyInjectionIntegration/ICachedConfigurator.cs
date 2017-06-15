
using System;

namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    internal interface ICachedConfigurator
    {
        void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider services);
    }
}