namespace MassTransit
{
    public interface AmazonSqsSendContext<out T> :
        AmazonSqsSendContext,
        SendContext<T>
        where T : class
    {
    }


    public interface AmazonSqsSendContext :
        SendContext
    {
        string GroupId { set; }
        string DeduplicationId { set; }
        int? DelaySeconds { set; }
    }
}
