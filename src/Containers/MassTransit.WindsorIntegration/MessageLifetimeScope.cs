namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel.Lifestyle.Scoped;


    public class MessageLifetimeScope :
        CallContextLifetimeScope
    {
        public MessageLifetimeScope(ConsumeContext consumeContext)
        {
            ConsumeContext = consumeContext;
        }

        public ConsumeContext ConsumeContext { get; }
    }
}
