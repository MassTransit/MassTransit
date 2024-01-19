namespace MassTransit
{
    using Mediator;
    using Mediator.Internals;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;


    public static class MediatorRequestExtensions
    {
        /// <summary>
        /// Sends a request, with the specified response type, and awaits the response.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="request">The request message</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">The response type</typeparam>
        /// <returns>The response object, which may be <see langword="null"/></returns>
        public static async Task<T> SendRequest<T>(this IMediator mediator, Request<T> request, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                var requestClient = mediator.CreateRequestClient<Request<T>>();
                var response = await requestClient.GetResponse<T, Null<T>>(request, cancellationToken).ConfigureAwait(false);

                if (response.Is<T>(out var defaultResponse))
                {
                    return defaultResponse.Message;
                }
            }
            catch (RequestException exception)
                when (exception.InnerException is { } innerException)
            {
                ExceptionDispatchInfo.Capture(innerException).Throw();
            }

            return default!;
        }
    }
}
