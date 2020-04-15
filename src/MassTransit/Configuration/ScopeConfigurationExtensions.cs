namespace MassTransit
{
    using Pipeline.Pipes;
    using Scoping;


    public static class ScopeConfigurationExtensions
    {
        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The send pipe configurator</param>
        /// <param name="scopeProvider">SendScopeProvider</param>
        public static void UseSendScope(this ISendPipelineConfigurator configurator, ISendScopeProvider scopeProvider)
        {
            configurator.ConfigureSend(cfg => cfg.ConnectSendPipeSpecificationObserver(new ScopeSendPipeSpecificationObserver(scopeProvider)));
        }

        /// <summary>
        /// Use scope for Send
        /// </summary>
        /// <param name="configurator">The publish pipe configurator</param>
        /// <param name="scopeProvider">PublishScopeProvider</param>
        public static void UsePublishScope(this IPublishPipelineConfigurator configurator, IPublishScopeProvider scopeProvider)
        {
            configurator.ConfigurePublish(cfg => cfg.ConnectPublishPipeSpecificationObserver(new ScopePublishPipeSpecificationObserver(scopeProvider)));
        }
    }
}
