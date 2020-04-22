namespace MassTransit.StructureMapIntegration
{
    using Scoping;
    using StructureMap;


    public static class StructureMapScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="context">IContext</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, IContext context)
        {
            configurator.UseSendScope(context.GetInstance<ISendScopeProvider>());
        }

        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="container">IContainer</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, IContainer container)
        {
            configurator.UseSendScope(container.GetInstance<ISendScopeProvider>());
        }

        /// <summary>
        /// Use scope for Publish
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="context">IContext</param>
        public static void UsePublishScope(this IBusFactoryConfigurator configurator, IContext context)
        {
            configurator.UsePublishScope(context.GetInstance<IPublishScopeProvider>());
        }

        /// <summary>
        /// Use scope for Publish
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="container">IContainer</param>
        public static void UsePublishScope(this IBusFactoryConfigurator configurator, IContainer container)
        {
            configurator.UsePublishScope(container.GetInstance<IPublishScopeProvider>());
        }
    }
}
