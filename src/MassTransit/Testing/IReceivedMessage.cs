namespace MassTransit.Testing
{
    using System;


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

        DateTime StartTime { get; }
        TimeSpan ElapsedTime { get; }

        Exception Exception { get; }
        Type MessageType { get; }
        string ShortTypeName { get; }
        object MessageObject { get; }
    }
}
