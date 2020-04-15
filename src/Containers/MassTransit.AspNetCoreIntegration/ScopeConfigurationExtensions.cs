namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using ScopeProviders;


    public static class ScopeConfigurationExtensions
    {
        /// <summary>
        /// Use HttpContext scope for send message, if HttpContext is not persists new scope will be created
        /// </summary>
        /// <param name="configurator">IBusFactoryConfigurator</param>
        /// <param name="serviceProvider">IServiceProvider</param>
        public static void UseHttpContextScope(this IBusFactoryConfigurator configurator, IServiceProvider serviceProvider)
        {
            configurator.UseSendScope(new AspNetCoreSendScopeProvider(serviceProvider));
            configurator.UsePublishScope(new AspNetCorePublishScopeProvider(serviceProvider));
        }
    }
}
