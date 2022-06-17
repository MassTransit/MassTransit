namespace MassTransit
{
    public interface IBusOutboxConfigurator
    {
        /// <summary>
        /// Disable the outbox message delivery service, removing the hosted service from the service collection
        /// </summary>
        void DisableDeliveryService();
    }
}
