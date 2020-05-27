namespace MassTransit.Registration
{
    public interface IBusAttachmentRegistrationConfigurator<out TContainerContext> :
        IRegistrationConfigurator
        where TContainerContext : class
    {
        /// <summary>
        /// Add the bus attachment to the container, configured properly
        /// </summary>
        /// <param name="busAttachmentFactory"></param>
        void SetBusAttachmentFactory<T>(T busAttachmentFactory)
            where T : IBusAttachmentRegistrationFactory<TContainerContext>;
    }
}
