namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel.Lifestyle.Scoped;


    public class MessageLifetimeScope :
        CallContextLifetimeScope
    {
        public ConsumeContext ConsumeContext { get; }

        public MessageLifetimeScope(ConsumeContext consumeContext)
        {
            ConsumeContext = consumeContext;
        }
    }
}
