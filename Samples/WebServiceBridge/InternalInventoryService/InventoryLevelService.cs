namespace InternalInventoryService
{
    using Inventory.Messages;
    using MassTransit.ServiceBus;

    public class InventoryLevelService :
        Consumes<QueryInventoryLevel>.All
    {
        public IServiceBus Bus { get; set; }

        public void Consume(QueryInventoryLevel message)
        {
            PartInventoryLevelStatus status = new PartInventoryLevelStatus(message.PartNumber, 10, 50);

            Bus.Publish(status);
        }
    }
}