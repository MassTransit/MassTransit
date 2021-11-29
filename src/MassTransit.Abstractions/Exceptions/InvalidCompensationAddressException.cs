namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class InvalidCompensationAddressException :
        ActivityExecutionException
    {
        public InvalidCompensationAddressException()
        {
        }

        public InvalidCompensationAddressException(Uri address)
            : base($"An invalid compensation address was specified: {address}")
        {
        }

        public InvalidCompensationAddressException(string message)
            : base(message)
        {
        }

        public InvalidCompensationAddressException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidCompensationAddressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
