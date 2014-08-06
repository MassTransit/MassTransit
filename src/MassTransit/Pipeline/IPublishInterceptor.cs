namespace MassTransit.Pipeline
{
    using System.Threading.Tasks;


    /// <summary>
    /// Allows a message to be intercepted during Publish
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPublishInterceptor<in T>
        where T : class, PipeContext
    {
        Task PrePublish(PublishContext<T> context);

        Task PreSend(PublishContext<T> context, SendContext<T> sendContext);

        Task PostSend(PublishContext<T> context, SendContext<T> sendContext);

        Task PostPublish(PublishContext<T> context);
    }
}