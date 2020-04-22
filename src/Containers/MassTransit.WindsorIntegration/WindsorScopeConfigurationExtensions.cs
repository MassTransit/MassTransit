namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Scoping;


    public static class WindsorScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="kernel">IKernel</param>
        public static void UseSendScope(this IBusFactoryConfigurator configurator, IKernel kernel)
        {
            configurator.UseSendScope(kernel.Resolve<ISendScopeProvider>());
        }

        /// <summary>
        /// Use scope for Publish
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="kernel">IKernel</param>
        public static void UsePublishScope(this IBusFactoryConfigurator configurator, IKernel kernel)
        {
            configurator.UsePublishScope(kernel.Resolve<IPublishScopeProvider>());
        }
    }
}
