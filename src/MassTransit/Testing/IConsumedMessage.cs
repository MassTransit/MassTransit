namespace MassTransit.Testing
{
    using System;


    public interface IConsumedMessage
    {
        ConsumeContext Context { get; }

        Exception Exception { get; }

        Type MessageType { get; }
    }


    public interface IConsumedMessage<out T> :
        IConsumedMessage
        where T : class
    {
        new ConsumeContext<T> Context { get; }
    }
}
