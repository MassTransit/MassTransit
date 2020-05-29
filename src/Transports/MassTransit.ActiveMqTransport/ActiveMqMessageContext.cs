namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;


    public interface ActiveMqMessageContext
    {
        IMessage TransportMessage { get; }

        IPrimitiveMap Properties { get; }
    }
}
