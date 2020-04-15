namespace MassTransit.LamarIntegration
{
    using Lamar;
    using Scoping;


    public static class LamarScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="serviceContext">IServiceContext</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, IServiceContext serviceContext)
        {
            configurator.UseSendScope(serviceContext.GetInstance<ISendScopeProvider>());
            configurator.UsePublishScope(serviceContext.GetInstance<IPublishScopeProvider>());
        }

        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="container">IContainer</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, IContainer container)
        {
            configurator.UseSendScope(container.GetInstance<ISendScopeProvider>());
            configurator.UsePublishScope(container.GetInstance<IPublishScopeProvider>());
        }
    }
}
