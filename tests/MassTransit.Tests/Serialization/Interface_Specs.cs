namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using MassTransit.Configuration;
    using MassTransit.Serialization;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture(typeof(NewtonsoftJsonMessageSerializer))]
    [TestFixture(typeof(SystemTextJsonMessageSerializer))]
    [TestFixture(typeof(BsonMessageSerializer))]
    [TestFixture(typeof(NewtonsoftXmlMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializer))]
    [TestFixture(typeof(EncryptedMessageSerializerV2))]
    public class Deserializing_an_interface :
        SerializationTest
    {
        [Test]
        public void Should_create_a_proxy_for_the_interface()
        {
            var user = new UserImpl("Chris", "noone@nowhere.com");
            ComplaintAdded complaint = new ComplaintAddedImpl(user, "No toilet paper", BusinessArea.Appearance)
            {
                Body = "There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine."
            };

            var result = SerializeAndReturn(complaint);

            complaint.Equals(result).ShouldBe(true);
        }

        [Test]
        public async Task Should_dispatch_an_interface_via_the_pipeline()
        {
            var pipe = new ConsumePipeSpecification().BuildConsumePipe();

            var consumer = new MultiTestConsumer(TestTimeout);
            consumer.Consume<ComplaintAdded>();

            consumer.Connect(pipe);

            var user = new UserImpl("Chris", "noone@nowhere.com");
            ComplaintAdded complaint = new ComplaintAddedImpl(user, "No toilet paper", BusinessArea.Appearance)
            {
                Body = "There was no toilet paper in the stall, forcing me to use my treasured issue of .NET Developer magazine."
            };


            await pipe.Send(new TestConsumeContext<ComplaintAdded>(complaint));

            consumer.Received.Select<ComplaintAdded>().Any().ShouldBe(true);
        }

        public Deserializing_an_interface(Type serializerType)
            : base(serializerType)
        {
        }
    }


    public interface ComplaintAdded
    {
        User AddedBy { get; }

        DateTime AddedAt { get; }

        string Subject { get; }

        string Body { get; }

        BusinessArea Area { get; }
    }


    public enum BusinessArea
    {
        Unknown = 0,
        Appearance,
        Courtesy
    }


    public interface User
    {
        string Name { get; }
        string Email { get; }
    }


    public class UserImpl : User
    {
        public UserImpl(string name, string email)
        {
            Name = name;
            Email = email;
        }

        protected UserImpl()
        {
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.Name, Name) && Equals(other.Email, Email);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!typeof(User).IsAssignableFrom(obj.GetType()))
                return false;
            return Equals((User)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Email != null ? Email.GetHashCode() : 0);
            }
        }
    }


    public class ComplaintAddedImpl :
        ComplaintAdded
    {
        public ComplaintAddedImpl(User addedBy, string subject, BusinessArea area)
        {
            var dateTime = DateTime.UtcNow;
            AddedAt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second,
                dateTime.Millisecond, DateTimeKind.Utc);

            AddedBy = addedBy;
            Subject = subject;
            Area = area;
            Body = string.Empty;
        }

        protected ComplaintAddedImpl()
        {
        }

        public User AddedBy { get; set; }

        public DateTime AddedAt { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public BusinessArea Area { get; set; }

        public bool Equals(ComplaintAdded other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return AddedBy.Equals(other.AddedBy) && other.AddedAt.Equals(AddedAt) && Equals(other.Subject, Subject) && Equals(other.Body, Body)
                && Equals(other.Area, Area);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!typeof(ComplaintAdded).GetTypeInfo().IsAssignableFrom(obj.GetType()))
                return false;
            return Equals((ComplaintAdded)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = AddedBy != null ? AddedBy.GetHashCode() : 0;
                result = (result * 397) ^ AddedAt.GetHashCode();
                result = (result * 397) ^ (Subject != null ? Subject.GetHashCode() : 0);
                result = (result * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                result = (result * 397) ^ Area.GetHashCode();
                return result;
            }
        }
    }
}
