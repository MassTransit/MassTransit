using System;

namespace MassTransit.EventStoreDbIntegration
{
    public sealed class StreamCategory
    {
        internal const string AllStreamName = "$all";

        public static readonly StreamCategory AllStream = new StreamCategory(AllStreamName);

        public static StreamCategory FromString(string streamCategory) => new StreamCategory(streamCategory);

        StreamCategory(string streamCategory)
        {
            if (string.IsNullOrWhiteSpace(streamCategory))
                throw new ArgumentException(nameof(streamCategory));

            Value = streamCategory;
            IsAllStream = Value.Equals(AllStreamName);
        }

        string Value { get; }
        public bool IsAllStream { get; }

        public override string ToString() => Value;

        public static implicit operator string(StreamCategory self) => self.ToString();
    }
}
