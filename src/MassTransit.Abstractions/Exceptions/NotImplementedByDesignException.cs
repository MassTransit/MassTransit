namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class NotImplementedByDesignException :
        MassTransitException
    {
        public NotImplementedByDesignException()
            : this("This method has not been implemented by design.")
        {
        }

        public NotImplementedByDesignException(string message)
            : base(message)
        {
        }

        public NotImplementedByDesignException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NotImplementedByDesignException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
