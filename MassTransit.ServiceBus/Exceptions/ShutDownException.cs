namespace MassTransit.ServiceBus.Exceptions
{
    using System;

    public class ShutDownException :
        Exception
    {
        public ShutDownException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}