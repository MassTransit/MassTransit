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

        bool? _isAllStream = null;

        StreamName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        string Value { get; }
        public bool IsAllStream => _isAllStream ??= Value.Equals(AllStreamName);

        public override string ToString() => Value;

        public static implicit operator string(StreamName self) => self.ToString();
    }
}
