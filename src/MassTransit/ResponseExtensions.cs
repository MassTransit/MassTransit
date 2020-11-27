namespace MassTransit
{
    public static class ResponseExtensions
    {
        /// <summary>
        /// Used for pattern matching via (response,message)
        /// </summary>
        /// <param name="response"></param>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public static void Deconstruct(this Response response, out Response context, out object message)
        {
            context = response;
            message = response.Message;
        }
    }
}
