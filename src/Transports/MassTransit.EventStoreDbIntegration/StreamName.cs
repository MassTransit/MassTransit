using System;

namespace MassTransit.EventStoreDbIntegration
{
    public sealed class StreamName
    {
        public static StreamName ForCheckpoint(string checkpointId, string prefix = null) =>
            prefix == null
                ? new StreamName($"checkpoint-{checkpointId}")
                : new StreamName($"[{prefix}]checkpoint-{checkpointId}");

        public static StreamName Custom(string streamName, string prefix = null) =>
            prefix == null
                ? new StreamName(streamName)
                : new StreamName($"[{prefix}]{streamName}");

        StreamName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            if (value == StreamCategory.AllStreamName)
                throw new ArgumentException("The '$all' stream is not a valid stream name. It is a stream category that can only be subscribed to.", nameof(value));

            Value = value;
        }

        string Value { get; }

        public override string ToString() => Value;

        public static implicit operator string(StreamName self) => self.ToString();
    }
}
