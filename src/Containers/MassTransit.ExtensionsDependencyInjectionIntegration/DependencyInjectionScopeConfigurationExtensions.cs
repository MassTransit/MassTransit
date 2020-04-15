namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping;


    public static class DependencyInjectionScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="serviceProvider">IServiceProvider</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, IServiceProvider serviceProvider)
        {
            configurator.UseSendScope(serviceProvider.GetRequiredService<ISendScopeProvider>());
            configurator.UsePublishScope(serviceProvider.GetRequiredService<IPublishScopeProvider>());
        }
    }
}
