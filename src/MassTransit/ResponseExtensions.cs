namespace MassTransit
{
    using System;
    using System.Linq;
    using Metadata;


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

        /// <summary>
        /// Returns true if the response type is explicitly accepted, or if the accept response header is
        /// not present (downlevel client).
        /// </summary>
        /// <param name="context">The consumed message context</param>
        /// <typeparam name="T">The response type</typeparam>
        /// <returns>True if explicitly support or header is missing, otherwise false</returns>
        public static bool IsResponseAccepted<T>(this ConsumeContext context)
            where T : class
        {
            string[] acceptTypes = context.Headers.Get(MessageHeaders.Request.Accept, default(string[]));
            if (acceptTypes == null || acceptTypes.Length <= 0)
                return true;

            string[] matchingTypeNames = TypeMetadataCache<T>.MessageTypeNames;

            return acceptTypes.Any(accept => matchingTypeNames.Any(x => x.Equals(accept, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
