namespace MassTransit.Registration
{
    public interface IBusAttachmentRegistrationFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstanceConfigurator CreateBusAttachment(IBusAttachmentRegistrationContext<TContainerContext> context);
    }
}
