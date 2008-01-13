using System;
using MassTransit.ServiceBus.Util;

namespace MassTransit.ServiceBus
{
    /// <summary>
    /// A public interface to the envelope containing message(s)
    /// </summary>
    public interface IEnvelope : ICloneable
    {
        /// <summary>
        /// The unique identifier of this envelope
        /// </summary>
        MessageId Id { get; set; }

        /// <summary>
        /// The unique identifier of the original envelope this envelope is in response to
        /// </summary>
        MessageId CorrelationId { get; set; }

        /// <summary>
        /// The return endpoint for the message(s) in the envelope
        /// </summary>
        IEndpoint ReturnEndpoint { get; }

        /// <summary>
        /// The messages contained in the envelope
        /// </summary>
        IMessage[] Messages { get; set; }

        /// <summary>
        /// The label stored on the envelope
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Indicates whether the message should be delivered in a recoverable method
        /// </summary>
        bool Recoverable { get; set; }

        /// <summary>
        /// Specifies the time before the envelope is no longer valid and should be discarded
        /// </summary>
        TimeSpan TimeToBeReceived { get; set; }

        /// <summary>
        /// The time the envelope was sent (only valid for received envelopes)
        /// </summary>
        DateTime SentTime { get; set; }

        /// <summary>
        /// The time the envelope arrived (only valid for received envelopes)
        /// </summary>
        DateTime ArrivedTime { get; set; }
    }
}