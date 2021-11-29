namespace MassTransit.Testing
{
    using System;


    public interface IPublishedMessage :
        IAsyncListElement
    {
        SendContext Context { get; }

        DateTime StartTime { get; }
        TimeSpan ElapsedTime { get; }

        Exception Exception { get; }
        Type MessageType { get; }
        string ShortTypeName { get; }
        object MessageObject { get; }
    }


    public interface IPublishedMessage<out T> :
        IPublishedMessage
        where T : class
    {
        new PublishContext<T> Context { get; }
    }
}
