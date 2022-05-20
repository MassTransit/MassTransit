#nullable enable
namespace MassTransit.Events
{
    using System;


    public class ReceiveTransportFaultedEvent :
        ReceiveTransportFaulted
    {
        public ReceiveTransportFaultedEvent(Uri inputAddress, Exception exception)
        {
            InputAddress = inputAddress;
            Exception = exception;
        }

        public Uri InputAddress { get; }

        public Exception? Exception { get; }
    }
}
