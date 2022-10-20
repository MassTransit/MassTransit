namespace MassTransit
{
    using Apache.NMS;


    public interface ActiveMqMessageContext
    {
        IMessage TransportMessage { get; }

        IPrimitiveMap Properties { get; }

        string GroupId { get; }
        int GroupSequence { get; }
    }
}
