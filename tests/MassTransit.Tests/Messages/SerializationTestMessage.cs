namespace MassTransit.Tests.Messages
{
    using System;


    [Serializable]
    public class SerializationTestMessage :
        IEquatable<SerializationTestMessage>
    {
        public Guid GuidValue { get; set; }
        public bool BoolValue { get; set; }
        public byte ByteValue { get; set; }
        public string StringValue { get; set; }
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public decimal DecimalValue { get; set; }
        public double DoubleValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public TimeSpan TimeSpanValue { get; set; }
        public decimal? MaybeMoney { get; set; }

        public bool Equals(SerializationTestMessage obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GuidValue.Equals(GuidValue) &&
                Equals(obj.StringValue, StringValue) &&
                obj.IntValue == IntValue &&
                obj.BoolValue == BoolValue &&
                obj.ByteValue == ByteValue &&
                obj.LongValue == LongValue &&
                obj.DecimalValue == DecimalValue &&
                obj.DoubleValue == DoubleValue &&
                obj.TimeSpanValue == TimeSpanValue &&
                obj.MaybeMoney == MaybeMoney &&
                obj.DateTimeValue.Equals(DateTimeValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(SerializationTestMessage))
                return false;
            return Equals((SerializationTestMessage)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = GuidValue.GetHashCode();
                result = (result * 397) ^ (StringValue != null ? StringValue.GetHashCode() : 0);
                result = (result * 397) ^ IntValue;
                result = (result * 397) ^ LongValue.GetHashCode();
                result = (result * 397) ^ BoolValue.GetHashCode();
                result = (result * 397) ^ ByteValue.GetHashCode();
                result = (result * 397) ^ DecimalValue.GetHashCode();
                result = (result * 397) ^ DoubleValue.GetHashCode();
                result = (result * 397) ^ DateTimeValue.GetHashCode();
                result = (result * 397) ^ MaybeMoney.GetHashCode();
                result = (result * 397) ^ TimeSpanValue.GetHashCode();
                return result;
            }
        }
    }
}
