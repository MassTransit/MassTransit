namespace MassTransit.Testing
{
    using System;
    using MessageObservers;


    public interface IPublishedMessage :
        IAsyncListElement
    {
        SendContext Context { get; }
        Exception Exception { get; }
        Type MessageType { get; }
        object MessageObject { get; }
    }


    public interface IPublishedMessage<out T> :
        IPublishedMessage
        where T : class
    {
        new PublishContext<T> Context { get; }
    }
}
