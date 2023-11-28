namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ConventionException :
        MassTransitException
    {
        public ConventionException()
        {
        }

        public ConventionException(string message)
            : base(message)
        {
        }

        public ConventionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ConventionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
