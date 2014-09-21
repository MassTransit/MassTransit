// Copyright 2007-2008 The Apache Software Foundation.
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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Context;
    using Magnum.TestFramework;
    using MassTransit.Serialization;
    using MassTransit.Services.Subscriptions.Messages;
    using NUnit.Framework;

    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(JsonMessageSerializer))]
//    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(VersionOneXmlMessageSerializer))]
    [TestFixture(typeof(BinaryMessageSerializer))]
    public class MoreSerialization_Specs<TSerializer> :
        SerializationSpecificationBase<TSerializer> where TSerializer : IMessageSerializer, new()
    {
        [Serializable]
        public class ContainerClass
        {
            public IList<OuterClass> Elements { get; set; }

            public bool Equals(ContainerClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                if (ReferenceEquals(other.Elements, Elements)) return true;
                if (other.Elements == null && Elements != null) return false;
                if (other.Elements != null && Elements == null) return false;

                if (other.Elements != null && Elements != null)
                {
                    if (other.Elements.Count != Elements.Count) return false;

                    for (int i = 0; i < Elements.Count; i++)
                    {
                        if (!Equals(other.Elements[i], Elements[i]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (ContainerClass)) return false;
                return Equals((ContainerClass) obj);
            }

            public override int GetHashCode()
            {
                return (Elements != null ? Elements.GetHashCode() : 0);
            }
        }

        [Serializable]
        public class DictionaryContainerClass
        {
            public IDictionary<string, OuterClass> Elements { get; set; }

            public DictionaryContainerClass()
            {
                Elements = new Dictionary<string, OuterClass>();
            }

            public bool Equals(DictionaryContainerClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                if (ReferenceEquals(other.Elements, Elements)) return true;
                if (other.Elements == null && Elements != null)
                {
                    Trace.WriteLine("other element was null");
                    return false;
                }
                if (other.Elements != null && Elements == null)
                {
                    Trace.WriteLine("other element was not null");
                    return false;
                }

                if (other.Elements != null && Elements != null)
                {
                    if (other.Elements.Count != Elements.Count) return false;

                    foreach (KeyValuePair<string, OuterClass> pair in Elements)
                    {
                        if (!other.Elements.ContainsKey(pair.Key))
                            return false;

                        if (!Equals(pair.Value, other.Elements[pair.Key]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(DictionaryContainerClass)) return false;
                return Equals((DictionaryContainerClass)obj);
            }

            public override int GetHashCode()
            {
                return (Elements != null ? Elements.GetHashCode() : 0);
            }
        }

        [Serializable]
        public class PrimitiveArrayClass
        {
            public PrimitiveArrayClass()
            {
                Values = new int[] {};
            }

            public int[] Values { get; set; }

            public bool Equals(PrimitiveArrayClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                if (ReferenceEquals(other.Values, Values)) return true;
                if (other.Values == null && Values != null) return false;
                if (other.Values != null && Values == null) return false;

                if (other.Values != null && Values != null)
                {
                    if (other.Values.Length != Values.Length) return false;

                    for (int i = 0; i < Values.Length; i++)
                    {
                        if (!Equals(other.Values[i], Values[i]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (PrimitiveArrayClass)) return false;
                return Equals((PrimitiveArrayClass) obj);
            }

            public override int GetHashCode()
            {
                return (Values != null ? Values.GetHashCode() : 0);
            }
        }

        [Serializable]
        public class GenericArrayClass<T>
        {
            public T[] Values { get; set; }

            public bool Equals(GenericArrayClass<T> other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                if (ReferenceEquals(other.Values, Values)) return true;
                if (other.Values == null && Values != null) return false;
                if (other.Values != null && Values == null) return false;

                if (other.Values != null && Values != null)
                {
                    if (other.Values.Length != Values.Length) return false;

                    for (int i = 0; i < Values.Length; i++)
                    {
                        if (!Equals(other.Values[i], Values[i]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(GenericArrayClass<T>)) return false;
                return Equals((GenericArrayClass<T>)obj);
            }

            public override int GetHashCode()
            {
                return (Values != null ? Values.GetHashCode() : 0);
            }
        }

        [Serializable]
        public class OuterClass
        {
            public InnerClass Inner { get; set; }

            public bool Equals(OuterClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Inner, Inner);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (OuterClass)) return false;
                return Equals((OuterClass) obj);
            }

            public override int GetHashCode()
            {
                return (Inner != null ? Inner.GetHashCode() : 0);
            }
        }

        [Serializable]
        public class InnerClass
        {
            public string Name { get; set; }

            public bool Equals(InnerClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Name, Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (InnerClass)) return false;
                return Equals((InnerClass) obj);
            }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }
        }


        [Serializable]
        public class EmptyClass
        {
            public bool Equals(EmptyClass other)
            {
                return !ReferenceEquals(null, other);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (EmptyClass)) return false;
                return Equals((EmptyClass) obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        [Test]
        public void Should_serialize_an_empty_message()
        {
            byte[] serializedMessageData;

            var serializer = new TSerializer();

            var message = new SubscriptionRefresh(Enumerable.Empty<SubscriptionInformation>());

            using (var output = new MemoryStream())
            {
                serializer.Serialize(output, new SendContext<SubscriptionRefresh>(message));

                serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            using (var input = new MemoryStream(serializedMessageData))
            {
                var receiveContext = ReceiveContext.FromBodyStream(input);
                serializer.Deserialize(receiveContext);

                IConsumeContext<SubscriptionRefresh> context;
                receiveContext.TryGetContext(out context).ShouldBeTrue();

                context.ShouldNotBeNull();

                context.Message.Subscriptions.Count.ShouldEqual(message.Subscriptions.Count);
            }
        }
        [Test]
        public void Should_serialize_a_message_with_one_list_item()
        {
            byte[] serializedMessageData;

            var serializer = new TSerializer();

            var message = new SubscriptionRefresh(new[]{new SubscriptionInformation(Guid.NewGuid(),1,typeof(object),new Uri("http://localhost/"))});

            using (var output = new MemoryStream())
            {
                serializer.Serialize(output, new SendContext<SubscriptionRefresh>(message));

                serializedMessageData = output.ToArray();

                Console.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
            }

            using (var input = new MemoryStream(serializedMessageData))
            {
                var receiveContext = ReceiveContext.FromBodyStream(input);
                serializer.Deserialize(receiveContext);

                IConsumeContext<SubscriptionRefresh> context;
                receiveContext.TryGetContext(out context).ShouldBeTrue();

                context.ShouldNotBeNull();

                context.Message.Subscriptions.Count.ShouldEqual(message.Subscriptions.Count);
            }
        }


        [Test]
        public void A_collection_of_objects_should_be_properly_serialized()
        {
            ContainerClass message = new ContainerClass
                {
                    Elements = new List<OuterClass>
                        {
                            new OuterClass
                                {
                                    Inner = new InnerClass {Name = "Chris"},
                                },
                            new OuterClass
                                {
                                    Inner = new InnerClass {Name = "David"},
                                },
                        }
                };

            TestSerialization(message);
        }

        [Test]
        public void A_dictionary_of_objects_should_be_properly_serialized()
        {
            DictionaryContainerClass message = new DictionaryContainerClass
                {
                    Elements = new Dictionary<string, OuterClass>
                        {
                            {"Chris", new OuterClass{Inner = new InnerClass {Name = "Chris"}}},
                            {"David", new OuterClass{Inner = new InnerClass {Name = "David"}}},
                        }
                };

            TestSerialization(message);
        }

        [Test]
        public void A_dictionary_of_one_objects_should_be_properly_serialized()
        {
            DictionaryContainerClass message = new DictionaryContainerClass
                {
                    Elements = new Dictionary<string, OuterClass>
                        {
                            {"David", new OuterClass{Inner = new InnerClass {Name = "David"}}},
                        }
                };

            TestSerialization(message);
        }

        [Test]
        public void A_dictionary_of_no_objects_should_be_properly_serialized()
        {
            DictionaryContainerClass message = new DictionaryContainerClass
                {
                    Elements = new Dictionary<string, OuterClass>
                        {
                        }
                };

            TestSerialization(message);
        }


        [Test]
        public void A_primitive_array_of_objects_should_be_properly_serialized()
        {
            PrimitiveArrayClass message = new PrimitiveArrayClass
                {
                    Values = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}

                };

            TestSerialization(message);
        }

        [Test]
        public void A_primitive_array_of_objects_with_one_element_should_be_properly_serialized()
        {
            var message = new PrimitiveArrayClass
            {
                Values = new[] { 1 }

            };

            TestSerialization(message);
        }

        [Test]
        public void A_primitive_array_of_objects_with_no_elements_should_be_properly_serialized()
        {
            var message = new PrimitiveArrayClass
            {
                Values = new int[] {  }
            };

            TestSerialization(message);
        }

        [Test]
        public void An_empty_class_should_not_break_the_mold()
        {
            EmptyClass message = new EmptyClass();

            TestSerialization(message);
        }

        [Test]
        public void A_private_setter_should_be_serializable()
        {
            const string expected = "Dr. Cox";

            PrivateSetter message = new PrivateSetter(expected);

            TestSerialization(message);
        }


        [Serializable]
        public class EnumClass
        {
            public SomeEnum Setting { get; set; }

            public bool Equals(EnumClass other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Setting, Setting);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (EnumClass)) return false;
                return Equals((EnumClass) obj);
            }

            public override int GetHashCode()
            {
                return Setting.GetHashCode();
            }
        }

        [Test]
        public void An_enumeration_should_be_serializable()
        {
            EnumClass message = new EnumClass {Setting = SomeEnum.Second};

            TestSerialization(message);
            
        }

        [Test]
        public void An_empty_array_of_objects_should_be_properly_serialized()
        {
            PrimitiveArrayClass message = new PrimitiveArrayClass
                {
                    Values = new int[] {}
                };

            TestSerialization(message);
        }

        [Test]
        public void An_array_of_objects_should_be_properly_serialized()
        {
            var message = new GenericArrayClass<InnerClass>
                {
                    Values = new[]
                        {
                            new InnerClass { Name = "Chris" },
                            new InnerClass { Name = "David" },
                        }
                };

            TestSerialization(message);
        }

        [Test]
        public void A_nested_object_should_be_properly_serialized()
        {
            OuterClass message = new OuterClass
                {
                    Inner = new InnerClass {Name = "Chris"},
                };

            TestSerialization(message);
        }
    }

        [Serializable]
    public class PrivateSetter
    {
        public PrivateSetter(string name)
        {
            Name = name;
        }

        protected PrivateSetter()
        {
        }

        public string Name { get; private set; }

        public bool Equals(PrivateSetter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PrivateSetter)) return false;
            return Equals((PrivateSetter) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }

    public enum SomeEnum
    {
        First,
        Second,
        Third,
    }
}