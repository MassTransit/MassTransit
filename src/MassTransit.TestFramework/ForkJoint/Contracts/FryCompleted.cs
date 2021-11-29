namespace MassTransit.TestFramework.ForkJoint.Contracts
{
    using System;


    public interface FryCompleted :
        OrderLineCompleted
    {
        Size Size { get; }
    }


    class FryCompletedResult :
        FryCompleted
    {
        public FryCompletedResult(DateTime created, DateTime completed, Guid orderId, Guid orderLineId, Size size, string description)
        {
            Created = created;
            Completed = completed;
            OrderId = orderId;
            OrderLineId = orderLineId;
            Description = description;
            Size = size;
        }

        public DateTime Created { get; }
        public DateTime Completed { get; }
        public Guid OrderId { get; }
        public Guid OrderLineId { get; }
        public Size Size { get; }
        public string Description { get; }
    }
}
