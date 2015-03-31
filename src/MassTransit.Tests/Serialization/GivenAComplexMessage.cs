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
namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Linq;
    using MassTransit.Serialization;
    using Messages;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    public class Given_a_variety_of_challenging_messages :
        SerializationTest
    {
        public SerializationTestMessage Message { get; private set; }

        public Given_a_variety_of_challenging_messages(Type serializerType)
            : base(serializerType)
        {
        }


        class C : IEquatable<C>
        {
            public byte[] Contents { get; set; }

            public bool Equals(C other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Contents.SequenceEqual(other.Contents);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((C)obj);
            }

            public override int GetHashCode()
            {
                return (Contents != null ? Contents.GetHashCode() : 0);
            }
        }


        class B : IEquatable<B>
        {
            public DateTime Local { get; set; }
            public DateTime Universal { get; set; }

            public bool Equals(B other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Local.Equals(other.Local) && Universal.Equals(other.Universal);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((B)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Local.GetHashCode() * 397) ^ Universal.GetHashCode();
                }
            }
        }


        class A : IEquatable<A>
        {
            public A(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; private set; }
            public string Value { get; private set; }

            public bool Equals(A other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != GetType())
                    return false;
                return Equals((A)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                }
            }
        }


        [Test]
        public void Byte_array()
        {
            TestSerialization(new C
            {
                Contents = new byte[] {0x56, 0x34, 0xf3}
            });
        }

        [Test]
        public void Crazy_date_time()
        {
            TestSerialization(new B
            {
                Local = new DateTime(2001, 9, 11, 8, 46, 30, DateTimeKind.Local),
                Universal = new DateTime(2001, 9, 11, 9, 3, 2, DateTimeKind.Local).ToUniversalTime(),
            });
        }

        [Test]
        public void No_default_constructor()
        {
            TestSerialization(new A("Dru", "Sellers"));
        }

        [Test]
        public void Serialize_complex()
        {
            Message = new SerializationTestMessage
            {
                DecimalValue = 123.45m,
                LongValue = 098123213,
                BoolValue = true,
                ByteValue = 127,
                IntValue = 123,
                DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
                TimeSpanValue = TimeSpan.FromSeconds(30),
                GuidValue = Guid.NewGuid(),
                StringValue = "Chris's Sample Code",
                DoubleValue = 1823.172,
                MaybeMoney = 567.89m,
            };

            TestSerialization(Message);
        }

        [Test]
        public void Serialize_small_number()
        {
            const decimal smallNumber = 0.000001M;

            TestSerialization(new SmallNumberMessage(){SmallNumber = smallNumber});
        }


        public class SmallNumberMessage : IEquatable<SmallNumberMessage>
        {
            public decimal SmallNumber { get; set; }

            public bool Equals(SmallNumberMessage other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return SmallNumber == other.SmallNumber;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != this.GetType())
                    return false;
                return Equals((SmallNumberMessage)obj);
            }

            public override int GetHashCode()
            {
                return SmallNumber.GetHashCode();
            }
        }
        

        [Test]
        public void Serialize_simple()
        {
            TestSerialization(new PingMessage());
        }
    }
}