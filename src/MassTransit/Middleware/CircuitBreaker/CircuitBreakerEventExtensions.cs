namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Threading.Tasks;
    using Contracts;


    public static class CircuitBreakerEventExtensions
    {
        /// <summary>
        /// Set the concurrency limit of the filter
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Task PublishCircuitBreakerOpened(this IPipe<EventContext> pipe, Exception exception)
        {
            return pipe.PublishEvent<CircuitBreakerOpened>(new Opened(exception));
        }

        /// <summary>
        /// Set the concurrency limit of the filter
        /// </summary>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public static Task PublishCircuitBreakerClosed(this IPipe<EventContext> pipe)
        {
            return pipe.PublishEvent<CircuitBreakerClosed>(new Closed());
        }


        class Closed :
            CircuitBreakerClosed
        {
        }


        class Opened :
            CircuitBreakerOpened
        {
            public Opened(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}
