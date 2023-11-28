namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;
    using System.Threading;


    [Serializable]
    public class RequestCanceledException :
        OperationCanceledException
    {
        public RequestCanceledException()
        {
        }

        public RequestCanceledException(string requestId, CancellationToken cancellationToken)
            : base(FormatMessage(requestId), cancellationToken)
        {
        }

        public RequestCanceledException(string requestId, Exception innerException, CancellationToken cancellationToken)
            : base(FormatMessage(requestId), innerException, cancellationToken)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected RequestCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static string FormatMessage(string requestId)
        {
            return $"The request was canceled, RequestId: {requestId}";
        }
    }
}
