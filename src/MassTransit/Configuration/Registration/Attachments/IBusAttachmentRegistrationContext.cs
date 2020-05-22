namespace MassTransit.Registration.Attachments
{
    public interface IBusAttachmentRegistrationContext<out TContainerContext> :
        IRegistration
        where TContainerContext : class
    {
        TContainerContext Container { get; }
    }
}
