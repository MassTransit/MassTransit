namespace MassTransit.Context.Converters
{
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Calls the generic version of the ISendEndpoint.Send method with the object's type
    /// </summary>
    public interface IResponseEndpointConverter
    {
        Task Respond(ConsumeContext consumeContext, object message);

        Task Respond(ConsumeContext consumeContext, object message, IPipe<SendContext> pipe);
    }
}
