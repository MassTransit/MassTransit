namespace MassTransit.ServiceBus.Exceptions
{
    using System;

    public class ConventionException :
        Exception
    {
        public ConventionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConventionException(string message) : base(message)
        {
        }
    }
}