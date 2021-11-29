namespace MassTransit.Tests.Middleware
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class IntentionalTestException :
        Exception
    {
        public IntentionalTestException()
        {
        }

        public IntentionalTestException(string message)
            : base(message)
        {
        }

        public IntentionalTestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IntentionalTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
