namespace MassTransit.ServiceBus.Saga
{
    using System;

    /// <summary>
    /// A marker interface that also defines the properties of a
    /// message involved in an MassTransit saga.
    /// </summary>
    public interface ISagaMessage : IMessage
    {
        /// <summary>
        /// Gets/sets the Id of the workflow the message is related to.
        /// </summary>
        Guid SagaId { get; set; }
    }
}