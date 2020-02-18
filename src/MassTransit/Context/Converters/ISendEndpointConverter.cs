namespace MassTransit.Context.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Calls the generic version of the ISendEndpoint.Send method with the object's type
    /// </summary>
    public interface ISendEndpointConverter
    {
        Task Send(ISendEndpoint endpoint, object message, CancellationToken cancellationToken = default);

        Task Send(ISendEndpoint endpoint, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default);
    }
}
