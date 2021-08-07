namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using MassTransit.Serialization;
    using NUnit.Framework;


    [TestFixture(typeof(XmlMessageSerializer))]
    [TestFixture(typeof(JsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class MoreSerialization_Specs :
        SerializationTest
    {
        [Test]
        public void A_collection_of_objects_should_be_properly_serialized()
        {
            var message = new ContainerClass
            {
                Elements = new List<OuterClass>
                {
                    new OuterClass {Inner = new InnerClass {Name = "Chris"}},
                    new OuterClass {Inner = new InnerClass {Name = "David"}}
                }
            };

            TestSerialization(message);
        }

        [Test]
        public void A_dictionary_of_no_objects_should_be_properly_serialized()
        {
            var message = new DictionaryContainerClass {Elements = new Dictionary<string, OuterClass>()};

            TestSerialization(message);
        }

        [Test]
        public void A_dictionary_of_objects_should_be_properly_serialized()
        {
            var message = new DictionaryContainerClass
            {
                Elements = new Dictionary<string, OuterClass>
                {
                    {"Chris", new OuterClass {Inner = new InnerClass {Name = "Chris"}}},
                    {"David", new OuterClass {Inner = new InnerClass {Name = "David"}}}
                }
            };

            TestSerialization(message);
        }

        [Test]
        public void A_dictionary_of_one_objects_should_be_properly_serialized()
        {
            var message = new DictionaryContainerClass
            {
                Elements = new Dictionary<string, OuterClass> {{"David", new OuterClass {Inner = new InnerClass {Name = "David"}}}}
            };

            TestSerialization(message);
        }

        [Test]
        public void A_hashset_of_integers_should_be_properly_serialized()
        {
            var message = new PrimitiveHashSetClass
            {
                Values =
                {
                    1,
                    2,
                    3
                }
            };

            TestSerialization(message);
        }

        [Test]
        public void A_nested_object_should_be_properly_serialized()
        {
            var message = new OuterClass {Inner = new InnerClass {Name = "Chris"}};

            TestSerialization(message);
        }

        [Test]
        public void A_primitive_array_of_objects_should_be_properly_serialized()
        {
            var message = new PrimitiveArrayClass {Values = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}};

            TestSerialization(message);
        }

        [Test]
        public void A_primitive_array_of_objects_with_no_elements_should_be_properly_serialized()
        {
            var message = new PrimitiveArrayClass {Values = new int[] { }};

            TestSerialization(message);
        }

        [Test]
        public void A_primitive_array_of_objects_with_one_element_should_be_properly_serialized()
        {
            var message = new PrimitiveArrayClass {Values = new[] {1}};

            TestSerialization(message);
        }

        [Test]
        public void A_private_setter_should_be_serializable()
        {
            const string expected = "Dr. Cox";

            var message = new PrivateSetter(expected);

            TestSerialization(message);
        }

        [Test]
        public void A_readonly_dictionary_list_message()
        {
            var message = new SucceededCommandResult(NewId.NextGuid());

            TestSerialization(message);
        }

        [Test]
        public void A_set_of_integers_should_be_properly_serialized()
        {
            var message = new PrimitiveSetClass
            {
                Values =
                {
                    1,
                    2,
                    3
                }
            };

            TestSerialization(message);
        }

        [Test]
        public void An_array_of_objects_should_be_properly_serialized()
        {
            var message = new GenericArrayClass<InnerClass> {Values = new[] {new InnerClass {Name = "Chris"}, new InnerClass {Name = "David"}}};

            TestSerialization(message);
        }

        [Test]
        public void An_empty_array_of_objects_should_be_properly_serialized()
        {
            var message = new PrimitiveArrayClass {Values = new int[] { }};

            TestSerialization(message);
        }

        [Test]
        public void An_empty_class_should_not_break_the_mold()
        {
            var message = new EmptyClass();

            TestSerialization(message);
        }

        [Test]
        public void An_enumeration_should_be_serializable()
        {
            var message = new EnumClass {Setting = SomeEnum.Second};

            TestSerialization(message);
        }


        [Serializable]
        public class ContainerClass
        {
            public IList<OuterClass> Elements { get; set; }

            public bool Equals(ContainerClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (ReferenceEquals(other.Elements, Elements))
                    return true;

                if (other.Elements == null && Elements != null)
                    return false;

                if (other.Elements != null && Elements == null)
                    return false;

                if (other.Elements != null && Elements != null)
                {
                    if (other.Elements.Count != Elements.Count)
                        return false;

                    for (var i = 0; i < Elements.Count; i++)
                    {
                        if (!Equals(other.Elements[i], Elements[i]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(ContainerClass))
                    return false;

                return Equals((ContainerClass)obj);
            }

            public override int GetHashCode()
            {
                return Elements != null ? Elements.GetHashCode() : 0;
            }
        }


        [Serializable]
        public class DictionaryContainerClass
        {
            public DictionaryContainerClass()
            {
                Elements = new Dictionary<string, OuterClass>();
            }

            public IDictionary<string, OuterClass> Elements { get; set; }

            public bool Equals(DictionaryContainerClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (ReferenceEquals(other.Elements, Elements))
                    return true;

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
                    if (other.Elements.Count != Elements.Count)
                        return false;

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
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(DictionaryContainerClass))
                    return false;

                return Equals((DictionaryContainerClass)obj);
            }

            public override int GetHashCode()
            {
                return Elements != null ? Elements.GetHashCode() : 0;
            }
        }


        public class PrimitiveHashSetClass
        {
            public PrimitiveHashSetClass()
            {
                Values = new HashSet<int>();
            }

            public HashSet<int> Values { get; set; }

            public bool Equals(PrimitiveHashSetClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (ReferenceEquals(other.Values, Values))
                    return true;

                if (other.Values == null && Values != null)
                    return false;

                if (other.Values != null && Values == null)
                    return false;

                if (other.Values != null && Values != null)
                {
                    if (other.Values.Count != Values.Count)
                        return false;

                    return other.Values.SetEquals(Values);
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(PrimitiveHashSetClass))
                    return false;

                return Equals((PrimitiveHashSetClass)obj);
            }

            public override int GetHashCode()
            {
                return Values != null ? Values.GetHashCode() : 0;
            }
        }


        public class PrimitiveSetClass
        {
            public PrimitiveSetClass()
            {
                Values = new HashSet<int>();
            }

            public ISet<int> Values { get; set; }

            public bool Equals(PrimitiveSetClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (ReferenceEquals(other.Values, Values))
                    return true;

                if (other.Values == null && Values != null)
                    return false;

                if (other.Values != null && Values == null)
                    return false;

                if (other.Values != null && Values != null)
                {
                    if (other.Values.Count != Values.Count)
                        return false;

                    return other.Values.SetEquals(Values);
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(PrimitiveSetClass))
                    return false;

                return Equals((PrimitiveSetClass)obj);
            }

            public override int GetHashCode()
            {
                return Values != null ? Values.GetHashCode() : 0;
            }
        }


        [Serializable]
        public class PrimitiveArrayClass
        {
            public PrimitiveArrayClass()
            {
                Values = new int[] { };
            }

            public int[] Values { get; set; }

            public bool Equals(PrimitiveArrayClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (ReferenceEquals(other.Values, Values))
                    return true;

                if (other.Values == null && Values != null)
                    return false;

                if (other.Values != null && Values == null)
                    return false;

                if (other.Values != null && Values != null)
                {
                    if (other.Values.Length != Values.Length)
                        return false;

                    for (var i = 0; i < Values.Length; i++)
                    {
                        if (!Equals(other.Values[i], Values[i]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(PrimitiveArrayClass))
                    return false;

                return Equals((PrimitiveArrayClass)obj);
            }

            public override int GetHashCode()
            {
                return Values != null ? Values.GetHashCode() : 0;
            }
        }


        [Serializable]
        public class GenericArrayClass<T>
        {
            public T[] Values { get; set; }

            public bool Equals(GenericArrayClass<T> other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                if (ReferenceEquals(other.Values, Values))
                    return true;

                if (other.Values == null && Values != null)
                    return false;

                if (other.Values != null && Values == null)
                    return false;

                if (other.Values != null && Values != null)
                {
                    if (other.Values.Length != Values.Length)
                        return false;

                    for (var i = 0; i < Values.Length; i++)
                    {
                        if (!Equals(other.Values[i], Values[i]))
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(GenericArrayClass<T>))
                    return false;

                return Equals((GenericArrayClass<T>)obj);
            }

            public override int GetHashCode()
            {
                return Values != null ? Values.GetHashCode() : 0;
            }
        }


        [Serializable]
        public class OuterClass
        {
            public InnerClass Inner { get; set; }

            public bool Equals(OuterClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                return Equals(other.Inner, Inner);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(OuterClass))
                    return false;

                return Equals((OuterClass)obj);
            }

            public override int GetHashCode()
            {
                return Inner != null ? Inner.GetHashCode() : 0;
            }
        }


        [Serializable]
        public class InnerClass
        {
            public string Name { get; set; }

            public bool Equals(InnerClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                return Equals(other.Name, Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(InnerClass))
                    return false;

                return Equals((InnerClass)obj);
            }

            public override int GetHashCode()
            {
                return Name != null ? Name.GetHashCode() : 0;
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
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(EmptyClass))
                    return false;

                return Equals((EmptyClass)obj);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }


        public MoreSerialization_Specs(Type serializerType)
            : base(serializerType)
        {
        }


        [Serializable]
        public class EnumClass
        {
            public SomeEnum Setting { get; set; }

            public bool Equals(EnumClass other)
            {
                if (ReferenceEquals(null, other))
                    return false;

                if (ReferenceEquals(this, other))
                    return true;

                return Equals(other.Setting, Setting);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != typeof(EnumClass))
                    return false;

                return Equals((EnumClass)obj);
            }

            public override int GetHashCode()
            {
                return Setting.GetHashCode();
            }
        }
    }


    public interface ICommandResult
    {
        bool Succeeded { get; }
        IValidationResult ValidationResult { get; }
        Guid CommandId { get; }
        object ReferenceId { get; }
    }


    public class ValidationResult :
        IValidationResult
    {
        public static readonly IValidationResult Succeeded = new ValidationResult(null);

        ValidationResult(string errorMessage, IReadOnlyDictionary<string, IReadOnlyList<string>> result = null)
        {
            ErrorMessage = errorMessage;
            Result = result ?? new Dictionary<string, IReadOnlyList<string>>();
        }

        ValidationResult(string errorMessage, Dictionary<string, List<string>> result)
            : this(errorMessage, result?.ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Value))
        {
        }

        public bool IsValid => string.IsNullOrEmpty(ErrorMessage) && Result?.Any() != true;
        public string ErrorMessage { get; }
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Result { get; }

        public static IValidationResult Error(string message)
        {
            return new ValidationResult(message);
        }

        public static IValidationResult Error(string key, string error)
        {
            return new ValidationResult(null, new Dictionary<string, List<string>>(1) {[key] = new List<string>(1) {error}});
        }
    }


    public class CommandResult :
        ICommandResult,
        IEquatable<CommandResult>
    {
        protected CommandResult(Guid commandId, IValidationResult validationResult, object referenceId)
        {
            ValidationResult = validationResult;
            ReferenceId = referenceId;
            CommandId = commandId;
        }

        public bool Succeeded => ValidationResult == null || ValidationResult.IsValid;
        public IValidationResult ValidationResult { get; }
        public object ReferenceId { get; }
        public Guid CommandId { get; }

        public bool Equals(CommandResult other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(ReferenceId, other.ReferenceId) && CommandId.Equals(other.CommandId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((CommandResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ReferenceId != null ? ReferenceId.GetHashCode() : 0) * 397) ^ CommandId.GetHashCode();
            }
        }

        public static bool operator ==(CommandResult left, CommandResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CommandResult left, CommandResult right)
        {
            return !Equals(left, right);
        }
    }


    public class SucceededCommandResult : CommandResult
    {
        public SucceededCommandResult(Guid commandId, object referenceId = null)
            : base(commandId, Serialization.ValidationResult.Succeeded, referenceId)
        {
        }
    }


    public interface IValidationResult
    {
        bool IsValid { get; }
        string ErrorMessage { get; }

        IReadOnlyDictionary<string, IReadOnlyList<string>> Result { get; }
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
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(PrivateSetter))
                return false;

            return Equals((PrivateSetter)obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }


    public enum SomeEnum
    {
        First,
        Second,
        Third
    }
}
