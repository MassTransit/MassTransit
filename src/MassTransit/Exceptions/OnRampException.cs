namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class OnRampException :
        MassTransitException
    {
        public OnRampException()
        {
        }

        public OnRampException(string message)
            : base(message)
        {
        }

        public OnRampException(string message, Exception innerException)
            :
            base(message, innerException)
        {
        }

        protected OnRampException(SerializationInfo info, StreamingContext context)
            :
            base(info, context)
        {
        }
    }
}
