namespace MassTransit.Internal
{
    using System;
    using Exceptions;

    public class NullEndpoint :
        IEndpoint
    {
        public void Dispose()
        {
            //do nothing
        }

        public Uri Uri
        {
            get { return new Uri("null://middleof/nowhere"); }
        }

        public void Send<T>(T message) where T : class
        {
            //do nothing
        }

        public void Send<T>(T message, TimeSpan timeToLive) where T : class
        {
            //do nothing
        }

        public object Receive(TimeSpan timeout)
        {
            return Receive(TimeSpan.Zero, null);
        }

        public object Receive(TimeSpan timeout, Predicate<object> accept)
        {
            throw new EndpointException(this, "NullEndpoints have no messages");
        }
    }
}