namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;


    public interface ActiveMqSendContext<out T> :
        ActiveMqSendContext,
        SendContext<T>
        where T : class
    {
    }


    public interface ActiveMqSendContext :
        SendContext
    {
        MsgPriority? Priority { set; }
    }
}
