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
        }

        /// <summary>
        /// Use scope for Publish
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="serviceProvider">IServiceProvider</param>
        public static void UsePublishScope(this IBusFactoryConfigurator configurator, IServiceProvider serviceProvider)
        {
            configurator.UsePublishScope(serviceProvider.GetRequiredService<IPublishScopeProvider>());
        }
    }
}
