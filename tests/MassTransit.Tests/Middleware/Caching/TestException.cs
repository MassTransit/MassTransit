namespace MassTransit.Tests.Middleware.Caching
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class TestException :
        Exception
    {
        public TestException()
        {
        }

        public TestException(string message)
            : base(message)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected TestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
