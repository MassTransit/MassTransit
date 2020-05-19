namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Primitives;


    public class NamespaceManagerSettings
    {
        public ITokenProvider TokenProvider { get; set; }
        public TimeSpan OperationTimeout { get; set; }
        public RetryPolicy RetryPolicy { get; set; }
    }
}