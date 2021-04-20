using System;

namespace MassTransit.EventStoreDbIntegration
{
    public sealed class StreamName
    {
        const string AllStreamName = "$all";

        public static readonly StreamName AllStream = new StreamName(AllStreamName);

        public static StreamName ForCategory(string streamCategory, string prefix = null) =>
            prefix == null
                ? new StreamName($"{streamCategory}-")
                : new StreamName($"[{prefix}]{streamCategory}-");

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

            Value = value;
            IsAllStream = value.Equals(AllStreamName);
        }

        string Value { get; }
        public bool IsAllStream { get; }

        public override string ToString() => Value;

        public static implicit operator string(StreamName self) => self.ToString();
    }
}
