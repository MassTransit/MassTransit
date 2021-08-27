using System;
using MassTransit.Serialization;

namespace MassTransit.Transports.OnRamp
{
    public class OnRampSerializedMessage :
        SerializedMessage
    {
        public Uri Destination { get; set; }
        public string ExpirationTime { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string Body { get; set; }
        public string MessageId { get; set; }
        public string ContentType { get; set; }
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string ConversationId { get; set; }
        public string InitiatorId { get; set; }
        public string HeadersAsJson { get; set; }
        public string PayloadMessageHeadersAsJson { get; set; }
    }
}
