namespace MassTransit
{
    using System;


    public interface IMessageEvent<T> :
        IMessageEvent
        where T : class
    {
    }


    public interface IMessageEvent :
        MessageContext
    {
        Type MessageType { get; }
    }
}