namespace MassTransit.Registration
{
    using Attachments;


    public interface IBusAttachmentRegistrationContext<out TContainerContext> :
        IRegistration
        where TContainerContext : class
    {
        TContainerContext Container { get; }

        void UseHealthCheck(IBusAttachmentFactoryConfigurator configurator);
    }
}
