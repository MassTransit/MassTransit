namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using MassTransit.Initializers;
    using NUnit.Framework;


    [TestFixture]
    public class Initializer_Specs
    {
        [Test]
        public async Task Should_copy_the_property_values()
        {
            var context = await MessageInitializerCache<TestInitializerMessage>.Initialize(new
            {
                StringValue = _stringValue,
                BoolValue = _boolValue,
                ByteValue = _byteValue,
                ShortValue = _shortValue,
                IntValue = _intValue,
                LongValue = _longValue,
                DoubleValue = _doubleValue,
                DecimalValue = _decimalValue,
                DateTimeValue = _dateTimeValue,
                DateTimeOffsetValue = _dateTimeOffsetValue,
                TimeSpanValue = _timeSpanValue,
                DayValue = _dayValue,
                ObjectValue = _objectValue,
            });

            var message = context.Message;

            Assert.That(message.StringValue, Is.EqualTo(_stringValue));
            Assert.That(message.BoolValue, Is.EqualTo(_boolValue));
            Assert.That(message.ByteValue, Is.EqualTo(_byteValue));
            Assert.That(message.ShortValue, Is.EqualTo(_shortValue));
            Assert.That(message.IntValue, Is.EqualTo(_intValue));
            Assert.That(message.LongValue, Is.EqualTo(_longValue));
            Assert.That(message.DoubleValue, Is.EqualTo(_doubleValue));
            Assert.That(message.DecimalValue, Is.EqualTo(_decimalValue));
            Assert.That(message.DateTimeValue, Is.EqualTo(_dateTimeValue));
            Assert.That(message.DateTimeOffsetValue, Is.EqualTo(_dateTimeOffsetValue));
            Assert.That(message.TimeSpanValue, Is.EqualTo(_timeSpanValue));
            Assert.That(message.DayValue, Is.EqualTo(_dayValue));
            Assert.That(message.ObjectValue, Is.EqualTo(_objectValue));
        }

        [Test]
        public async Task Should_copy_the_property_values_from_strings()
        {
            var context = await MessageInitializerCache<TestInitializerMessage>.Initialize(new
            {
                StringValue = _stringValue,
                BoolValue = _boolValue.ToString(),
                ByteValue = _byteValue.ToString(),
                ShortValue = _shortValue.ToString(),
                IntValue = _intValue.ToString(),
                LongValue = _longValue.ToString(),
                DoubleValue = _doubleValue.ToString(CultureInfo.InvariantCulture),
                DecimalValue = _decimalValue.ToString(CultureInfo.InvariantCulture),
                DateTimeValue = _dateTimeValue.ToString("O"),
                DateTimeOffsetValue = _dateTimeOffsetValue.ToString("O"),
                TimeSpanValue = _timeSpanValue.ToString("c"),
                DayValue = _dayValue.ToString(),
                ObjectValue = _objectValue,
            });

            var message = context.Message;

            Assert.That(message.StringValue, Is.EqualTo(_stringValue));
            Assert.That(message.BoolValue, Is.EqualTo(_boolValue));
            Assert.That(message.ByteValue, Is.EqualTo(_byteValue));
            Assert.That(message.ShortValue, Is.EqualTo(_shortValue));
            Assert.That(message.IntValue, Is.EqualTo(_intValue));
            Assert.That(message.LongValue, Is.EqualTo(_longValue));
            Assert.That(message.DoubleValue, Is.EqualTo(_doubleValue));
            Assert.That(message.DecimalValue, Is.EqualTo(_decimalValue));
            Assert.That(message.DateTimeValue, Is.EqualTo(_dateTimeValue));
            Assert.That(message.DateTimeOffsetValue, Is.EqualTo(_dateTimeOffsetValue));
            Assert.That(message.TimeSpanValue, Is.EqualTo(_timeSpanValue));
            Assert.That(message.DayValue, Is.EqualTo(_dayValue));
            Assert.That(message.ObjectValue, Is.EqualTo(_objectValue));
        }

        [Test]
        public async Task Should_copy_the_property_values_from_nullable_types()
        {
            var context = await MessageInitializerCache<TestInitializerMessage>.Initialize(new
            {
                StringValue = _stringValue,
                BoolValue = (bool?)_boolValue,
                ByteValue = (byte?)_byteValue,
                ShortValue = (short?)_shortValue,
                IntValue = (int?)_intValue,
                LongValue = (long?)_longValue,
                DoubleValue = (double?)_doubleValue,
                DecimalValue = (decimal?)_decimalValue,
                DateTimeValue = (DateTime?)_dateTimeValue,
                DateTimeOffsetValue = (DateTimeOffset?)_dateTimeOffsetValue,
                TimeSpanValue = (TimeSpan?)_timeSpanValue,
                DayValue = (Day?)_dayValue,
            });

            var message = context.Message;

            Assert.That(message.StringValue, Is.EqualTo(_stringValue));
            Assert.That(message.BoolValue, Is.EqualTo(_boolValue));
            Assert.That(message.ByteValue, Is.EqualTo(_byteValue));
            Assert.That(message.ShortValue, Is.EqualTo(_shortValue));
            Assert.That(message.IntValue, Is.EqualTo(_intValue));
            Assert.That(message.LongValue, Is.EqualTo(_longValue));
            Assert.That(message.DoubleValue, Is.EqualTo(_doubleValue));
            Assert.That(message.DecimalValue, Is.EqualTo(_decimalValue));
            Assert.That(message.DateTimeValue, Is.EqualTo(_dateTimeValue));
            Assert.That(message.DateTimeOffsetValue, Is.EqualTo(_dateTimeOffsetValue));
            Assert.That(message.TimeSpanValue, Is.EqualTo(_timeSpanValue));
            Assert.That(message.DayValue, Is.EqualTo(_dayValue));
        }

        [Test]
        public async Task Should_copy_the_property_values_to_nullable_types()
        {
            var context = await MessageInitializerCache<TestNullableMessage>.Initialize(new
            {
                StringValue = _stringValue,
                BoolValue = _boolValue,
                ByteValue = _byteValue,
                ShortValue = _shortValue,
                IntValue = _intValue,
                LongValue = _longValue,
                DoubleValue = _doubleValue,
                DecimalValue = _decimalValue,
                DateTimeValue = _dateTimeValue,
                DateTimeOffsetValue = _dateTimeOffsetValue,
                TimeSpanValue = _timeSpanValue,
                DayValue = _dayValue,
            });

            var message = context.Message;


            Assert.That(message.BoolValue, Is.EqualTo(_boolValue));
            Assert.That(message.ByteValue, Is.EqualTo(_byteValue));
            Assert.That(message.ShortValue, Is.EqualTo(_shortValue));
            Assert.That(message.IntValue, Is.EqualTo(_intValue));
            Assert.That(message.LongValue, Is.EqualTo(_longValue));
            Assert.That(message.DoubleValue, Is.EqualTo(_doubleValue));
            Assert.That(message.DecimalValue, Is.EqualTo(_decimalValue));
            Assert.That(message.DateTimeValue, Is.EqualTo(_dateTimeValue));
            Assert.That(message.DateTimeOffsetValue, Is.EqualTo(_dateTimeOffsetValue));
            Assert.That(message.TimeSpanValue, Is.EqualTo(_timeSpanValue));
            Assert.That(message.DayValue, Is.EqualTo(_dayValue));
        }

        [Test]
        public async Task Should_convert_value_types_to_strings()
        {
            var context = await MessageInitializerCache<TestStringMessage>.Initialize(new {Text = _intValue,});

            Assert.That(context.Message.Text, Is.EqualTo(_intValue.ToString()));
        }

        readonly bool _boolValue = true;
        readonly string _stringValue = "Hello";
        readonly byte _byteValue = 123;
        readonly short _shortValue = 12345;
        readonly int _intValue = 1234567;
        readonly long _longValue = 12345678L;
        readonly double _doubleValue = 867.5309;
        readonly decimal _decimalValue = 123.45m;
        readonly DateTime _dateTimeValue = new DateTime(2001, 2, 3, 4, 5, 6, 7);
        readonly DateTimeOffset _dateTimeOffsetValue = new DateTimeOffset(2001, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(-8));
        readonly TimeSpan _timeSpanValue = new TimeSpan(427, 1, 2, 3, 4);
        readonly Day _dayValue = Day.Tuesday;
        readonly object _objectValue = new Uri("loopback://localhost/");


        public enum Day
        {
            Sunday = 0,
            Monday = 1,
            Tuesday = 2,
        }


        public interface TestNullableMessage
        {
            bool? BoolValue { get; }
            byte? ByteValue { get; }
            short? ShortValue { get; }
            int? IntValue { get; }
            long? LongValue { get; }
            double? DoubleValue { get; }
            decimal? DecimalValue { get; }
            DateTime? DateTimeValue { get; }
            DateTimeOffset? DateTimeOffsetValue { get; }
            TimeSpan? TimeSpanValue { get; }
            Day? DayValue { get; }
        }


        public interface TestStringMessage
        {
            string Text { get; }
        }


        public interface TestInitializerMessage
        {
            string StringValue { get; }
            bool BoolValue { get; }
            byte ByteValue { get; }
            short ShortValue { get; }
            int IntValue { get; }
            long LongValue { get; }
            double DoubleValue { get; }
            decimal DecimalValue { get; }
            DateTime DateTimeValue { get; }
            DateTimeOffset DateTimeOffsetValue { get; }
            TimeSpan TimeSpanValue { get; }
            Day DayValue { get; }
            object ObjectValue { get; }
        }
    }
}
