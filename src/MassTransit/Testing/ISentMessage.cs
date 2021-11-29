namespace MassTransit.Testing
{
    using System;


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

        DateTime StartTime { get; }
        TimeSpan ElapsedTime { get; }

        Exception Exception { get; }
        Type MessageType { get; }
        string ShortTypeName { get; }
        object MessageObject { get; }
    }
}
