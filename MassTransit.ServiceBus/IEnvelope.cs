using System;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A public interface to the envelope containing message(s)
    /// </summary>
    public interface IEnvelope : ICloneable
    {
        MessageId Id { get; set; }

        MessageId CorrelationId { get; set; }

        string Label { get; set; }

        /// <summary>
        /// The return endpoint for the message(s) in the envelope
        /// </summary>
        IEndpoint ReturnEndpoint { get; }

        /// <summary>
        /// The messages contained in the envelope
        /// </summary>
        IMessage[] Messages { get; set; }

        bool Recoverable { get; set; }

        DateTime ArrivedTime { get; set; }
        DateTime SentTime { get; set; }

        TimeSpan TimeToBeReceived { get; set; }
    }
}