namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class RequestTimeoutException :
        RequestException
    {
        public RequestTimeoutException()
        {
        }

        public RequestTimeoutException(string requestId)
            : base(FormatMessage(requestId))
        {
        }

        public RequestTimeoutException(string requestId, Exception innerException)
            : base(FormatMessage(requestId), innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected RequestTimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static string FormatMessage(string requestId)
        {
            return $"Timeout waiting for response, RequestId: {requestId}";
        }
    }
}
