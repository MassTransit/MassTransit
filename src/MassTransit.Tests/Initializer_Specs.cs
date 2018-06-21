// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
using NUnit.Framework;


namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Initializers;


    [TestFixture]
    public class Initializer_Specs
    {
        readonly string _stringValue = "Hello";
        readonly byte _byteValue = 123;
        readonly short _shortValue = 12345;
        readonly int _intValue = 1234567;
        readonly long _longValue = 12345678L;
        readonly double _doubleValue = 867.5309;
        readonly decimal _decimalValue = 123.45m;
        readonly DateTime _dateTimeValue = new DateTime(2001, 2, 3, 4, 5, 6);
        readonly DateTimeOffset _dateTimeOffsetValue = new DateTimeOffset(2001, 1, 2, 3, 4, 5, 6, TimeSpan.FromHours(-8));

        [Test]
        public async Task Should_copy_the_property_values()
        {
            var message = await MessageInitializerCache<TestInitializerMessage>.Initialize(new
            {
                StringValue = _stringValue,
                ByteValue = _byteValue,
                ShortValue = _shortValue,
                IntValue = _intValue,
                LongValue = _longValue,
                DoubleValue = _doubleValue,
                DecimalValue = _decimalValue,
                DateTimeValue = _dateTimeValue,
                DateTimeOffsetValue = _dateTimeOffsetValue,
            });

            Assert.That(message.StringValue, Is.EqualTo(_stringValue));
            Assert.That(message.ByteValue, Is.EqualTo(_byteValue));
            Assert.That(message.ShortValue, Is.EqualTo(_shortValue));
            Assert.That(message.IntValue, Is.EqualTo(_intValue));
            Assert.That(message.LongValue, Is.EqualTo(_longValue));
            Assert.That(message.DoubleValue, Is.EqualTo(_doubleValue));
            Assert.That(message.DecimalValue, Is.EqualTo(_decimalValue));
            Assert.That(message.DateTimeValue, Is.EqualTo(_dateTimeValue));
            Assert.That(message.DateTimeOffsetValue, Is.EqualTo(_dateTimeOffsetValue));
        }


        public interface TestInitializerMessage
        {
            string StringValue { get; }
            byte ByteValue { get; }
            short ShortValue { get; }
            int IntValue { get; }
            long LongValue { get; }
            double DoubleValue { get; }
            decimal DecimalValue { get; }
            DateTime DateTimeValue { get; }
            DateTimeOffset DateTimeOffsetValue { get; }
        }
    }
}