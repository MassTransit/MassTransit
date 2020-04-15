namespace MassTransit.SimpleInjectorIntegration
{
    using Scoping;
    using SimpleInjector;


    public static class SimpleInjectorScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="container">Container</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, Container container)
        {
            configurator.UseSendScope(container.GetInstance<ISendScopeProvider>());
            configurator.UsePublishScope(container.GetInstance<IPublishScopeProvider>());
        }
    }
}
