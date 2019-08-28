namespace MassTransit.Conductor.Client
{
    using Clients;
    using GreenPipes;


    public interface IServiceClient :
        IAsyncDisposable
    {
        /// <summary>
        /// Create a request send endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRequestSendEndpoint<T> CreateRequestSendEndpoint<T>()
            where T : class;

        /// <summary>
        /// Create a request send endpoint with the specified consumeContext
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRequestSendEndpoint<T> CreateRequestSendEndpoint<T>(ConsumeContext consumeContext)
            where T : class;
    }
}
