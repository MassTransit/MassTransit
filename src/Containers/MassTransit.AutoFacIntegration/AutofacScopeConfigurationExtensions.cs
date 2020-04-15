namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using Scoping;


    public static class AutofacScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// /// <param name="lifetimeScope">IServiceProvider</param>
        public static void UseSendScope(this ISendPipelineConfigurator configurator, ILifetimeScope lifetimeScope)
        {
            configurator.UseSendScope(lifetimeScope.Resolve<ISendScopeProvider>());
        }

        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="componentContext">IComponentContext</param>
        public static void UseSendScope(this ISendPipelineConfigurator configurator, IComponentContext componentContext)
        {
            configurator.UseSendScope(componentContext.Resolve<ILifetimeScope>());
        }

        /// <summary>
        /// Use scope for Publish
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="lifetimeScope">IServiceProvider</param>
        public static void UsePublishScope(this IPublishPipelineConfigurator configurator, ILifetimeScope lifetimeScope)
        {
            configurator.UsePublishScope(lifetimeScope.Resolve<IPublishScopeProvider>());
        }

        /// <summary>
        /// Use scope for Publish
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="componentContext">IComponentContext</param>
        public static void UsePublishScope(this IPublishPipelineConfigurator configurator, IComponentContext componentContext)
        {
            configurator.UsePublishScope(componentContext.Resolve<ILifetimeScope>());
        }
    }
}
