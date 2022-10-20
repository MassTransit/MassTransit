namespace MassTransit
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
        string GroupId { set; }
        int? GroupSequence { set; }
    }
}
