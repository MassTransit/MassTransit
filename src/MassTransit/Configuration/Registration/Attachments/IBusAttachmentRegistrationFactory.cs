namespace MassTransit.Registration.Attachments
{
    public interface IBusAttachmentRegistrationFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstanceConfigurator CreateBusAttachment(IBusAttachmentRegistrationContext<TContainerContext> context);
    }
}
