using System;

namespace MassTransit.EventStoreDbIntegration.Tests
{
    public class OrderCreated
    {
        public OrderCreated(Guid orderId, Guid buyerId, DateTime createdOn)
        {
            OrderId = orderId;
            BuyerId = buyerId;
            CreatedOn = createdOn;
        }

        public Guid OrderId { get; }
        public Guid BuyerId { get; }
        public DateTime CreatedOn { get; }
    }

    public class OrderItemAdded
    {
        public OrderItemAdded(Guid orderId, Guid itemId)
        {
            OrderId = orderId;
            ItemId = itemId;
        }

        public Guid OrderId { get; }
        public Guid ItemId { get; }
    }

    public class OrderItemRemoved
    {
        public OrderItemRemoved(Guid orderId, Guid itemId)
        {
            OrderId = orderId;
            ItemId = itemId;
        }

        public Guid OrderId { get; }
        public Guid ItemId { get; }
    }

    public class OrderPlaced
    {
        public OrderPlaced(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }

    public class OrderCancelled
    {
        public OrderCancelled(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}
