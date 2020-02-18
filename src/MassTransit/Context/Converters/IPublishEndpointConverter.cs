namespace MassTransit.Context.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Calls the generic version of the IPublishEndpoint.Send method with the object's type
    /// </summary>
    public interface IPublishEndpointConverter
    {
        Task Publish(IPublishEndpoint endpoint, object message, CancellationToken cancellationToken = default);

        Task Publish(IPublishEndpoint endpoint, object message, IPipe<PublishContext> pipe, CancellationToken cancellationToken = default);
    }
}
