namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class CommandException :
        MassTransitException
    {
        public CommandException()
        {
        }

        public CommandException(string message)
            : base(message)
        {
        }

        public CommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected CommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
