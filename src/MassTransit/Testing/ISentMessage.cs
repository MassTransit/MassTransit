namespace MassTransit.Testing
{
    using System;
    using MessageObservers;


    public interface ISentMessage<out TMessage> :
        ISentMessage
        where TMessage : class
    {
        new SendContext<TMessage> Context { get; }
    }


    public interface ISentMessage :
        IAsyncListElement
    {
        SendContext Context { get; }

        Exception Exception { get; }

        Type MessageType { get; }

        object MessageObject { get; }
    }
}
