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
			throw new EndpointException(this, "NullEndpoints have no messages");
		}

        public object Receive(TimeSpan timeout, Predicate<object> accept)
        {
            throw new EndpointException(this, "NullEndpoints have no messages");
        }

    	public void Receive(TimeSpan timeout, Func<object, Func<object, bool>, bool> receiver)
    	{
			throw new EndpointException(this, "NullEndpoints have no messages");
		}
    }
}