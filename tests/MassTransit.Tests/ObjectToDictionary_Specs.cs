// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;
    using System.Collections.Generic;
    using GreenPipes.Internals.Mapping;
    using GreenPipes.Internals.Reflection;
    using NUnit.Framework;


    [TestFixture]
    public class Creating_a_dictionary_from_an_interface
    {
        [Test]
        public void Should_handle_a_dictionary_of_strings()
        {
            Assert.IsTrue(_dictionary.ContainsKey("Strings"));

            object value = _dictionary["Strings"];

            Assert.IsInstanceOf<object[]>(value);

            var values = value as object[];

            Assert.AreEqual(2, values.Length);
        }

        [Test]
        public void Should_handle_a_dictionary_of_values()
        {
            Assert.IsTrue(_dictionary.ContainsKey("StringSubValues"));

            object value = _dictionary["StringSubValues"];

            Assert.IsInstanceOf<object[]>(value);

            var values = value as object[];

            Assert.AreEqual(2, values.Length);
        }

        [Test]
        public void Should_handle_a_present_nullable_value()
        {
            Assert.IsTrue(_dictionary.ContainsKey("NullableDecimalValue"));
        }

        [Test]
        public void Should_handle_a_string()
        {
            Assert.IsTrue(_dictionary.ContainsKey("StringValue"));
        }

        [Test]
        public void Should_handle_an_array_of_objects()
        {
            Assert.IsTrue(_dictionary.ContainsKey("SubValues"));

            object value = _dictionary["SubValues"];

            Assert.IsInstanceOf<IDictionary<string, object>[]>(value);
        }

        [Test]
        public void Should_handle_an_array_of_strings()
        {
            Assert.IsTrue(_dictionary.ContainsKey("Names"));

            object value = _dictionary["Names"];

            Assert.IsInstanceOf<string[]>(value);
        }

        [Test]
        public void Should_handle_an_empty_nullable_value()
        {
            Assert.IsFalse(_dictionary.ContainsKey("NullableValue"));
        }

        [Test]
        public void Should_handle_an_int()
        {
            Assert.IsTrue(_dictionary.ContainsKey("IntValue"));
        }

        [Test]
        public void Should_handle_datetime()
        {
            Assert.IsTrue(_dictionary.ContainsKey("DateTimeValue"));
        }

        [Test]
        public void Should_round_trip_successfully()
        {
            var factory = new DynamicObjectConverterCache(new DynamicImplementationBuilder());

            IObjectConverter converter = factory.GetConverter(typeof(Values));

            var values = (Values)converter.GetObject(_dictionary);

            Assert.IsNotNull(values);

            Assert.AreEqual(_expected.DateTimeValue, values.DateTimeValue);
            Assert.AreEqual(_expected.IntValue, values.IntValue);
            Assert.AreEqual(_expected.NullableValue, values.NullableValue);
            Assert.AreEqual(_expected.NullableDecimalValue, values.NullableDecimalValue);
            Assert.AreEqual(_expected.Names.Length, values.Names.Length);
            Assert.AreEqual(_expected.Names[0], values.Names[0]);
            Assert.AreEqual(_expected.Names[1], values.Names[1]);
            Assert.AreEqual(_expected.Names[2], values.Names[2]);

            Assert.AreEqual(_expected.SubValues.Length, values.SubValues.Length);
            Assert.AreEqual(_expected.SubValues[0].Text, values.SubValues[0].Text);

            Assert.AreEqual(_expected.Strings, values.Strings);

            Assert.AreEqual(_expected.StringSubValues.Count, values.StringSubValues.Count);
            Assert.AreEqual(_expected.StringSubValues["A"].Text, values.StringSubValues["A"].Text);
            Assert.AreEqual(_expected.StringSubValues["B"].Text, values.StringSubValues["B"].Text);
        }

        IDictionary<string, object> _dictionary;
        ValuesImpl _expected;

        [OneTimeSetUp]
        public void Setup()
        {
            var factory = new DictionaryConverterCache();

            IDictionaryConverter converter = factory.GetConverter(typeof(ValuesImpl));

            _expected = new ValuesImpl("Hello", 27, new DateTime(2012, 10, 1), null, 123.45m);
            _dictionary =
                converter.GetDictionary(_expected);
        }


        public interface Values
        {
            string StringValue { get; }
            int IntValue { get; }
            DateTime DateTimeValue { get; }
            long? NullableValue { get; }
            decimal? NullableDecimalValue { get; }
            string[] Names { get; }

            SubValue[] SubValues { get; }

            IDictionary<string, string> Strings { get; }
            IDictionary<string, SubValue> StringSubValues { get; }
        }


        public interface SubValue
        {
            string Text { get; }
        }


        class ValuesImpl :
            Values
        {
            readonly DateTime _dateTimeValue;
            readonly int _intValue;
            readonly string[] _names;
            readonly decimal? _nullableDecimalValue;
            readonly long? _nullableValue;
            readonly IDictionary<string, SubValue> _stringSubValues;
            readonly string _stringValue;
            readonly IDictionary<string, string> _strings;
            readonly SubValue[] _subValues;

            public ValuesImpl(string stringValue, int intValue, DateTime dateTimeValue, long? nullableValue,
                decimal? nullableDecimalValue)
            {
                _stringValue = stringValue;
                _intValue = intValue;
                _dateTimeValue = dateTimeValue;
                _nullableValue = nullableValue;
                _nullableDecimalValue = nullableDecimalValue;
                _names = new[] {"A", "B", "C"};
                _subValues = new SubValue[]
                {
                    new SubValueImpl("A")
                };
                _strings = new Dictionary<string, string>
                {
                    {"A", "Aye"},
                    {"B", "Bee"},
                };
                _stringSubValues = new Dictionary<string, SubValue>
                {
                    {"A", new SubValueImpl("Eh")},
                    {"B", new SubValueImpl("Be")},
                };
            }

            public IDictionary<string, SubValue> StringSubValues
            {
                get { return _stringSubValues; }
            }

            public IDictionary<string, string> Strings
            {
                get { return _strings; }
            }

            public string StringValue
            {
                get { return _stringValue; }
            }

            public int IntValue
            {
                get { return _intValue; }
            }

            public DateTime DateTimeValue
            {
                get { return _dateTimeValue; }
            }

            public long? NullableValue
            {
                get { return _nullableValue; }
            }

            public decimal? NullableDecimalValue
            {
                get { return _nullableDecimalValue; }
            }

            public string[] Names
            {
                get { return _names; }
            }

            public SubValue[] SubValues
            {
                get { return _subValues; }
            }


            class SubValueImpl : SubValue,
                IEquatable<SubValueImpl>
            {
                readonly string _text;

                public SubValueImpl(string text)
                {
                    _text = text;
                }

                public bool Equals(SubValueImpl other)
                {
                    if (ReferenceEquals(null, other))
                        return false;
                    if (ReferenceEquals(this, other))
                        return true;
                    return string.Equals(_text, other._text);
                }

                public string Text
                {
                    get { return _text; }
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj))
                        return false;
                    if (ReferenceEquals(this, obj))
                        return true;
                    if (obj.GetType() != GetType())
                        return false;
                    return Equals((SubValueImpl)obj);
                }

                public override int GetHashCode()
                {
                    return (_text != null ? _text.GetHashCode() : 0);
                }

                public static bool operator ==(SubValueImpl left, SubValueImpl right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(SubValueImpl left, SubValueImpl right)
                {
                    return !Equals(left, right);
                }
            }
        }
    }
}