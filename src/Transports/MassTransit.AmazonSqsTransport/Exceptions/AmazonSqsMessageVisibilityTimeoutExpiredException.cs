using System;
using System.Runtime.Serialization;

namespace MassTransit
{
    [Serializable]
    public class AmazonSqsMessageVisibilityTimeoutExpiredException : TransportException
    {

        public AmazonSqsMessageVisibilityTimeoutExpiredException()
        {
        }

        public AmazonSqsMessageVisibilityTimeoutExpiredException(Uri uri)
            : base(uri)
        {
        }

        public AmazonSqsMessageVisibilityTimeoutExpiredException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public AmazonSqsMessageVisibilityTimeoutExpiredException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }

        protected AmazonSqsMessageVisibilityTimeoutExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
