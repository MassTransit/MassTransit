namespace MassTransit
{
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Mediator;


    public static class MediatorRequestExtensions
    {
        /// <summary>
        /// Sends a request, with the specified response type, and awaits the response.
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="request">The request message</param>
        /// <typeparam name="T">The response type</typeparam>
        /// <returns>The response object</returns>
        public static async Task<T> SendRequest<T>(this IMediator mediator, Request<T> request)
            where T : class
        {
            try
            {
                Response<T> response = await mediator.CreateRequest(request).GetResponse<T>().ConfigureAwait(false);

                return response.Message;
            }
            catch (RequestException exception)
            {
                var dispatchInfo = ExceptionDispatchInfo.Capture(exception.InnerException);

                dispatchInfo.Throw();

                throw;
            }
        }
    }
}
