namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public abstract class AbstractUriException :
        MassTransitException
    {
        protected AbstractUriException()
        {
        }

        protected AbstractUriException(Uri uri)
        {
            Uri = uri;
        }

        protected AbstractUriException(Uri uri, string message)
            : base($"{uri} => {message}")
        {
            Uri = uri;
        }

        protected AbstractUriException(Uri uri, string message, Exception innerException)
            : base($"{uri} => {message}", innerException)
        {
            Uri = uri;
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected AbstractUriException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Uri? Uri { get; protected set; }
    }
}
