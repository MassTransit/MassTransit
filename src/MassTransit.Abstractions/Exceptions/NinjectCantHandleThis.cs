namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class NinjectCantHandleThis :
        MassTransitException
    {
        public NinjectCantHandleThis()
            : this("This method has not been implemented by design.")
        {
        }

        public NinjectCantHandleThis(string message)
            : base(message)
        {
        }

        public NinjectCantHandleThis(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected NinjectCantHandleThis(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
