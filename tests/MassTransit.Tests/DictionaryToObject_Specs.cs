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
    public class Converting_a_dictionary_to_an_object
    {
        [Test]
        public void Should_include_a_string()
        {
            Assert.AreEqual("Hello", _values.StringValue);
        }

        [Test]
        public void Should_include_nullable_long()
        {
            Assert.IsTrue(_values.LongValue.HasValue);
            Assert.AreEqual(123, _values.LongValue.Value);
        }

        [Test]
        public void Should_include_nullable_timespan()
        {
            Assert.IsTrue(_values.TimeSpanValue.HasValue);
            Assert.AreEqual(TimeSpan.FromSeconds(5), _values.TimeSpanValue.Value);
        }

        [Test]
        public void Should_include_nullable_timespan_as_string()
        {
            Assert.IsTrue(_values.TimeSpanValueAsString.HasValue);
            Assert.AreEqual(TimeSpan.FromMilliseconds(20), _values.TimeSpanValueAsString.Value);
        }

        [Test]
        public void Should_include_the_bag_of_dicts()
        {
            Assert.IsNotNull(_values.BagOfDicts);
            Assert.AreEqual(2, _values.BagOfDicts.Count);

            Assert.IsTrue(_values.BagOfDicts.ContainsKey("First"));
            Assert.AreEqual("One", _values.BagOfDicts["First"]);
            Assert.IsTrue(_values.BagOfDicts.ContainsKey("Second"));
            Assert.AreEqual("Two", _values.BagOfDicts["Second"]);
        }

        [Test]
        public void Should_include_the_enum()
        {
            Assert.AreEqual(ValueType.Integer, _values.ValueType);
        }

        [Test]
        public void Should_include_the_enum_as_a_string()
        {
            Assert.AreEqual(ValueType.String, _values.ValueTypeAsString);
        }

        [Test]
        public void Should_include_the_enum_as_an_integer()
        {
            Assert.AreEqual(ValueType.Integer, _values.ValueTypeAsInt);
        }

        [Test]
        public void Should_include_the_integer()
        {
            Assert.AreEqual(27, _values.IntValue);
        }

        [Test]
        public void Should_include_the_list_of_sub_values()
        {
            Assert.IsNotNull(_values.ListOfSubValues);
            Assert.AreEqual(2, _values.ListOfSubValues.Count);

            Assert.AreEqual("A", _values.ListOfSubValues[0].A);
            Assert.AreEqual("B", _values.ListOfSubValues[0].B);

            Assert.AreEqual("1", _values.ListOfSubValues[1].A);
            Assert.AreEqual("2", _values.ListOfSubValues[1].B);
        }

        [Test]
        public void Should_include_the_sub_value()
        {
            Assert.IsNotNull(_values.SubValue);

            Assert.AreEqual("A", _values.SubValue.A);
            Assert.AreEqual("B", _values.SubValue.B);
        }

        [Test]
        public void Should_include_the_sub_values()
        {
            Assert.IsNotNull(_values.SubValues);
            Assert.AreEqual(2, _values.SubValues.Length);

            Assert.AreEqual("A", _values.SubValues[0].A);
            Assert.AreEqual("B", _values.SubValues[0].B);

            Assert.AreEqual("1", _values.SubValues[1].A);
            Assert.AreEqual("2", _values.SubValues[1].B);
        }

        IDictionary<string, object> _dictionary;
        Values _values;

        [OneTimeSetUp]
        public void Setup()
        {
            _dictionary = new Dictionary<string, object>
            {
                {"IntValue", 27},
                {"StringValue", "Hello"},
                {"LongValue", (long?)123},
                {"TimeSpanValue", TimeSpan.FromSeconds(5)},
                {"TimeSpanValueAsString", "00:00:00.0200000"},
                {"ValueType", ValueType.Integer},
                {"ValueTypeAsInt", 2},
                {"ValueTypeAsString", "String"},
                {
                    "SubValue", new Dictionary<string, object>
                    {
                        {"A", "A"},
                        {"B", "B"}
                    }
                },
                {"StringValues", new object[] {"A", "B", "C"}},
                {
                    "SubValues", new object[]
                    {
                        new Dictionary<string, object>
                        {
                            {"A", "A"},
                            {"B", "B"}
                        },
                        new Dictionary<string, object>
                        {
                            {"A", "1"},
                            {"B", "2"}
                        }
                    }
                },
                {
                    "ListOfSubValues", new object[]
                    {
                        new Dictionary<string, object>
                        {
                            {"A", "A"},
                            {"B", "B"}
                        },
                        new Dictionary<string, object>
                        {
                            {"A", "1"},
                            {"B", "2"}
                        }
                    }
                },
                {
                    "BagOfDicts", new object[]
                    {
                        new object[] {"First", "One"},
                        new object[] {"Second", "Two"},
                    }
                }
            };


            var converterCache = new DynamicObjectConverterCache(new DynamicImplementationBuilder());

            _values = (Values)converterCache.GetConverter(typeof(Values)).GetObject(_dictionary);
        }


        public interface Values
        {
            int IntValue { get; }
            string StringValue { get; }
            long? LongValue { get; }
            TimeSpan? TimeSpanValue { get; }
            TimeSpan? TimeSpanValueAsString { get; }
            ValueType ValueType { get; }
            ValueType ValueTypeAsInt { get; }
            ValueType ValueTypeAsString { get; }
            SubValue SubValue { get; }
            string[] StringValues { get; }
            SubValue[] SubValues { get; }
            IList<SubValue> ListOfSubValues { get; }
            IDictionary<string, string> BagOfDicts { get; }
        }


        public interface SubValue
        {
            string A { get; }
            string B { get; }
        }


        public enum ValueType
        {
            Default,
            String,
            Integer
        }
    }
}