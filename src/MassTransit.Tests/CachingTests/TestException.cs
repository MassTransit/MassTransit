namespace MassTransit.Tests.CachingTests
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

#if !NETCORE
        protected TestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        public TestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}