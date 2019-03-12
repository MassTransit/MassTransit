namespace MassTransit.AmazonSqsTransport
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
        string GroupId { get; set; }
        string DeduplicationId { get; set; }
    }
}
