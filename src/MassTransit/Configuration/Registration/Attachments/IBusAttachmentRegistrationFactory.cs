namespace MassTransit.Registration.Attachments
{
    public interface IBusAttachmentRegistrationFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstanceConfigurator CreateBusInstanceConfigurator(IRegistrationContext<TContainerContext> context);
    }
}
