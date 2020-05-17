namespace MassTransit.Testing
{
    using System;
    using MessageObservers;


    public interface IReceivedMessage<out T> :
        IReceivedMessage
        where T : class
    {
        new ConsumeContext<T> Context { get; }
    }


    public interface IReceivedMessage :
        IAsyncListElement
    {
        ConsumeContext Context { get; }

        Exception Exception { get; }

        Type MessageType { get; }

        object MessageObject { get; }
    }
}
