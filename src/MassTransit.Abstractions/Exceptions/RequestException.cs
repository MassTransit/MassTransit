namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class RequestException :
        MassTransitException
    {
        public RequestException(string message, Exception innerException, object response)
            : base(message, innerException)
        {
            Response = response;
        }

        public RequestException()
        {
        }

        public RequestException(string message)
            : base(message)
        {
        }

        public RequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        protected RequestException(string message, object response)
            : base(message)
        {
            Response = response;
        }

        public object? Response { get; }
    }
}
