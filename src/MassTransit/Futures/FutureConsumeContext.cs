namespace MassTransit.Futures
{
    using System;
    using Automatonymous;
    using MassTransit;


    public interface FutureConsumeContext<out TMessage> :
        FutureConsumeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
    }


    public interface FutureConsumeContext :
        ConsumeEventContext<FutureState>
    {
        Guid FutureId { get; }
    }
}