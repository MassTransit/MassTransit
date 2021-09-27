namespace MassTransit
{
    using System;
    using GreenPipes;


    public interface IRequestPipeConfigurator<TRequest> :
        IPipeConfigurator<SendContext<TRequest>>
        where TRequest : class
    {
        /// <summary>
        /// The RequestId assigned to the request, and used in the header for the outgoing request message
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// Set the request message time to live, which by default is equal to the request timeout. Clearing this value
        /// will prevent any TimeToLive value from being specified.
        /// </summary>
        RequestTimeout TimeToLive { set; }
    }
}
