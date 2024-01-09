#nullable enable
namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class SqlTopologyException :
        MassTransitException
    {
        public SqlTopologyException()
        {
        }

        public SqlTopologyException(string? message)
            : base(message)
        {
        }

        public SqlTopologyException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

    #if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
    #endif
        protected SqlTopologyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
