namespace InternalInventoryService
{
    using System;
    using Inventory.Messages;
    using MassTransit;

    public class InventoryLevelService :
        Consumes<QueryInventoryLevel>.All
    {
        public IServiceBus Bus { get; set; }

        public void Consume(QueryInventoryLevel message)
        {
            var status = new PartInventoryLevelStatus(message.PartNumber, DateTime.Now.Minute, DateTime.Now.Second);

            Bus.Publish(status);
        }
    }
}