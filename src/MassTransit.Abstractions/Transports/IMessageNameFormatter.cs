namespace MassTransit.Transports
{
    using System;


    /// <summary>
    /// Used to format a message type into a MessageName, which can be used as a valid
    /// queue name on the transport
    /// </summary>
    public interface IMessageNameFormatter
    {
        MessageName GetMessageName(Type type);
    }
}
