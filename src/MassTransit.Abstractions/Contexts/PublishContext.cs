namespace MassTransit
{
    public interface PublishContext :
        SendContext
    {
        /// <summary>
        /// True if the message must be delivered to a subscriber
        /// </summary>
        bool Mandatory { get; set; }
    }


    public interface PublishContext<out T> :
        SendContext<T>,
        PublishContext
        where T : class
    {
    }
}
