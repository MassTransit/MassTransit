namespace MassTransit
{
    using System;


    public interface SqlSendContext<out T> :
        SqlSendContext,
        SendContext<T>
        where T : class
    {
    }


    public interface SqlSendContext :
        SendContext,
        RoutingKeySendContext,
        PartitionKeySendContext
    {
        Guid TransportMessageId { get; }

        public short? Priority { set; }
    }
}
