namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PipelineException :
        MassTransitException
    {
        public PipelineException()
        {
        }

        public PipelineException(string message)
            : base(message)
        {
        }

        public PipelineException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }

        protected PipelineException(SerializationInfo info, StreamingContext context)
            :
            base(info, context)
        {
        }
    }
}
