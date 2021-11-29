namespace MassTransit
{
    using System;


    /// <summary>
    /// Specifies that a class implementing ISaga consumes TMessage as part of the saga
    /// </summary>
    /// <typeparam name="TMessage">The type of message to consume</typeparam>
    public interface InitiatedByOrOrchestrates<in TMessage> :
        IConsumer<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
    }
}
