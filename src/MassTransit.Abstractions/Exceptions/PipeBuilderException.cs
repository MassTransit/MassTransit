namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PipeFactoryException :
        MassTransitException
    {
        public PipeFactoryException()
        {
        }

        public PipeFactoryException(string message)
            : base(message)
        {
        }

        public PipeFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PipeFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
