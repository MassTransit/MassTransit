namespace MassTransit.AspNetCoreIntegration
{
    using System;
    using ScopeProviders;


    public static class AspNetCoreScopeConfigurationExtensions
    {
        /// <summary>
        /// Use HttpContext scope for send / publish message, if HttpContext is not exists new scope will be created
        /// </summary>
        /// <param name="configurator">IBusFactoryConfigurator</param>
        /// <param name="serviceProvider">IServiceProvider</param>
        public static void UseHttpContextSendScope(this IBusFactoryConfigurator configurator, IServiceProvider serviceProvider)
        {
            configurator.UseSendScope(new AspNetCoreSendScopeProvider(serviceProvider));
            configurator.UsePublishScope(new AspNetCorePublishScopeProvider(serviceProvider));
        }
    }
}
