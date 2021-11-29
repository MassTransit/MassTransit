namespace MassTransit.Tests.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using InitializerTestMessages;
    using MassTransit.Initializers;
    using Metadata;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Creating_a_message_via_an_initializer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initialize_the_properties()
        {
            IRequestClient<SimpleRequest> client = CreateRequestClient<SimpleRequest>();

            Response<SimpleResponse> response = await client.GetResponse<SimpleResponse>(new {Name = "Hello"});

            Assert.That(response.Message.Name, Is.EqualTo("Hello"));
            Assert.That(response.Message.Value, Is.EqualTo("World"));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<SimpleRequest>(async context =>
            {
                await context.RespondAsync<SimpleResponse>(new {Value = "World"});
            });
        }
    }


    [TestFixture]
    public class Creating_a_message_with_the_same_type_included :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initialize_the_properties()
        {
            InitializeContext<ExceptionInfo> context = await MessageInitializerCache<ExceptionInfo>.Initialize(new
            {
                Message = "Hello",
                ExceptionType = TypeCache<ArgumentException>.ShortName
            });

            var message = context.Message;

            Assert.That(message.Message, Is.EqualTo("Hello"));
            Assert.That(message.ExceptionType, Is.EqualTo(TypeCache<ArgumentException>.ShortName));
        }
    }


    [TestFixture]
    public class Creating_a_message_via_an_initializer_with_missing_properties :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initialize_the_properties()
        {
            IRequestClient<SimpleRequest> client = CreateRequestClient<SimpleRequest>();

            Response<SimpleResponse> response = await client.GetResponse<SimpleResponse>(new {Name = "Hello"});

            Assert.That(response.Message.Name, Is.EqualTo("Hello"));
            Assert.That(response.Message.Value, Is.Null);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<SimpleRequest>(async context =>
            {
                await context.RespondAsync<SimpleResponse>(new { });
            });
        }
    }


    [TestFixture]
    public class Creating_a_complex_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initialize_the_properties()
        {
            IRequestClient<ComplexRequest> client = CreateRequestClient<ComplexRequest>();

            Response<ComplexResponse> response = await client.GetResponse<ComplexResponse>(new
            {
                Name = "Hello",
                IntValue = 27,
                NullableIntValue = 42
            });

            Assert.That(response.Message.Name, Is.EqualTo("Hello"));
            Assert.That(response.Message.IntValue.HasValue, Is.True);
            Assert.That(response.Message.IntValue.Value, Is.EqualTo(27));
            Assert.That(response.Message.NullableIntValue, Is.EqualTo(42));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<ComplexRequest>(async context =>
            {
                await context.RespondAsync<ComplexResponse>(new { });
            });
        }
    }


    [TestFixture]
    public class Creating_a_super_complex_message :
        InMemoryTestFixture
    {
        [Test]
        public void Should_handle_basic_dictionary()
        {
            Assert.That(_response.Message.Strings, Is.Not.Null);
            Assert.That(_response.Message.Strings.Count, Is.EqualTo(2));
            Assert.That(_response.Message.Strings["Hello"], Is.EqualTo("World"));
            Assert.That(_response.Message.Strings["Thank You"], Is.EqualTo("Next"));
        }

        [Test]
        public void Should_handle_conversion_dictionary()
        {
            Assert.That(_response.Message.IntToStrings, Is.Not.Null);
            Assert.That(_response.Message.IntToStrings.Count, Is.EqualTo(2));
            Assert.That(_response.Message.IntToStrings[100], Is.EqualTo("1000"));
            Assert.That(_response.Message.IntToStrings[200], Is.EqualTo("2000"));
        }

        [Test]
        public void Should_handle_datetime_utc()
        {
            Assert.That(_response.Message.DateTimeValue, Is.EqualTo(_now));
        }

        [Test]
        public void Should_handle_decimal()
        {
            Assert.That(_response.Message.Amount, Is.EqualTo(867.53m));
        }

        [Test]
        public void Should_handle_duplicate_input_property_names()
        {
            Assert.That(_response.Message.NewProperty, Is.Not.Null);
            Assert.That(_response.Message.NewProperty.NewProperty, Is.EqualTo("Hello"));
        }

        [Test]
        public void Should_handle_enumerable_decimal()
        {
            Assert.That(_response.Message.Amounts, Is.Not.Null);
            Assert.That(_response.Message.Amounts.Count, Is.EqualTo(2));
            Assert.That(_response.Message.Amounts[0], Is.EqualTo(98.6m));
            Assert.That(_response.Message.Amounts[1], Is.EqualTo(98.6m));
        }

        [Test]
        public void Should_handle_enums()
        {
            Assert.That(_response.Message.EngineStatus, Is.EqualTo(Status.Started));
            Assert.That(_response.Message.NumberStatus, Is.EqualTo(Status.Stopped));
            Assert.That(_response.Message.StringStatus, Is.EqualTo(Status.Started));
        }

        [Test]
        public void Should_handle_exception()
        {
            Assert.That(_response.Message.Exception, Is.Not.Null);
            Assert.That(_response.Message.Exception.ExceptionType, Is.EqualTo(TypeCache<IntentionalTestException>.ShortName));
        }

        [Test]
        public async Task Should_handle_id_variable()
        {
            Assert.That(_response.Message.CorrelationId, Is.EqualTo(_response.Message.Id));
            Assert.That(_response.Message.StringId, Is.EqualTo(_response.Message.Id));
        }

        [Test]
        public void Should_handle_int()
        {
            Assert.That(_response.Message.IntValue, Is.EqualTo(27));
        }

        [Test]
        public void Should_handle_int_to_string_array()
        {
            Assert.That(_response.Message.Numbers, Is.Not.Null);
            Assert.That(_response.Message.Numbers.Length, Is.EqualTo(3));
            Assert.That(_response.Message.Numbers[0], Is.EqualTo("12"));
            Assert.That(_response.Message.Numbers[1], Is.EqualTo("24"));
            Assert.That(_response.Message.Numbers[2], Is.EqualTo("36"));
        }

        [Test]
        public void Should_handle_interface_type()
        {
            Assert.That(_response.Message.SubValue, Is.Not.Null);
            Assert.That(_response.Message.SubValue.Text, Is.EqualTo("Mary"));
        }

        [Test]
        public void Should_handle_interface_type_array()
        {
            Assert.That(_response.Message.SubValues.Length, Is.EqualTo(2));
            Assert.That(_response.Message.SubValues[0].Text, Is.EqualTo("Frank"));
            Assert.That(_response.Message.SubValues[1].Text, Is.EqualTo("Lola"));
        }

        [Test]
        public void Should_handle_lists()
        {
            Assert.That(_response.Message.StringList, Is.Not.Null);
            Assert.That(_response.Message.StringList.Count, Is.EqualTo(2));
            Assert.That(_response.Message.StringList[0], Is.EqualTo("Frank"));
            Assert.That(_response.Message.StringList[1], Is.EqualTo("Estelle"));
        }

        [Test]
        public void Should_handle_long_from_nullable()
        {
            Assert.That(_response.Message.NotNullableValue, Is.EqualTo(69));
        }

        [Test]
        public void Should_handle_nullable_decimal()
        {
            Assert.That(_response.Message.NullableDecimalValue, Is.EqualTo(123.45m));
        }

        [Test]
        public void Should_handle_nullable_long()
        {
            Assert.That(_response.Message.NullableValue, Is.EqualTo(42));
        }

        [Test]
        public void Should_handle_object_dictionary()
        {
            Assert.That(_response.Message.StringSubValues, Is.Not.Null);
            Assert.That(_response.Message.StringSubValues.Count, Is.EqualTo(2));
            Assert.That(_response.Message.StringSubValues["A"].Text, Is.EqualTo("Eh"));
            Assert.That(_response.Message.StringSubValues["B"].Text, Is.EqualTo("Bee"));
        }

        [Test]
        public void Should_handle_string()
        {
            Assert.That(_response.Message.StringValue, Is.EqualTo("Hello"));
        }

        [Test]
        public void Should_handle_string_array()
        {
            Assert.That(_response.Message.Names, Is.Not.Null);
            Assert.That(_response.Message.Names.Length, Is.EqualTo(3));
            Assert.That(_response.Message.Names[0], Is.EqualTo("Curly"));
            Assert.That(_response.Message.Names[1], Is.EqualTo("Larry"));
            Assert.That(_response.Message.Names[2], Is.EqualTo("Moe"));
        }

        [Test]
        public void Should_handle_task_of_task_of_int()
        {
            Assert.That(_response.Message.AsyncValue, Is.EqualTo(37));
        }

        [Test]
        public void Should_handle_timestamp_variable()
        {
            Assert.That(_response.Message.Timestamp.HasValue, Is.True);
            Assert.That(_response.Message.Timestamp, Is.GreaterThanOrEqualTo(_now));
        }

        [Test]
        public void Should_handle_uris()
        {
            Assert.That(_response.Message.ServiceAddress, Is.EqualTo(new Uri("http://masstransit-project.com")));
            Assert.That(_response.Message.OtherAddress, Is.EqualTo(new Uri("http://github.com")));
            Assert.That(_response.Message.StringAddress, Is.EqualTo("loopback://localhost/"));
        }

        DateTime _now;
        Response<SuperComplexResponse> _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _now = DateTime.UtcNow;

            IRequestClient<SuperComplexRequest> client = CreateRequestClient<SuperComplexRequest>();

            _response = await client.GetResponse<SuperComplexResponse>(new
            {
                InVar.CorrelationId,
                InVar.Id,
                InVar.Timestamp,
                StringId = InVar.Id,
                StringValue = "Hello",
                IntValue = 27,
                DateTimeValue = _now,
                NullableValue = 42,
                NotNullableValue = (int?)69,
                NullableDecimalValue = 123.45m,
                Numbers = new[] {12, 24, 36},
                Names = new[] {"Curly", "Larry", "Moe"},
                Exception = new IntentionalTestException("It Happens"),
                SubValue = new {Text = "Mary"},
                SubValues = new object[] {new {Text = "Frank"}, new {Text = "Lola"}},
                Amount = 867.53m,
                Amounts = Enumerable.Repeat(98.6m, 2),
                AsyncValue = GetIntResult().Select(x => x.Number),
                EngineStatus = Status.Started,
                NumberStatus = 12,
                StringStatus = "Started",
                ServiceAddress = new Uri("http://masstransit-project.com"),
                OtherAddress = "http://github.com",
                StringAddress = new Uri("loopback://localhost"),
                StringList = new[] {"Frank", "Estelle"},
                NewProperty = new SubProperty {NewProperty = "Hello"},
                Strings = new Dictionary<string, string>
                {
                    {"Hello", "World"},
                    {"Thank You", "Next"}
                },
                StringSubValues = new Dictionary<string, object>
                {
                    {"A", new {Text = "Eh"}},
                    {"B", new {Text = "Bee"}}
                },
                IntToStrings = new Dictionary<int, long>
                {
                    {100, 1000},
                    {200, 2000}
                }
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<SuperComplexRequest>(async context =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(context.ReceiveContext.GetBody()));

                await context.RespondAsync<SuperComplexResponse>(new { });
            });
        }

        async Task<(Task<int> Number, string Text)> GetIntResult()
        {
            return (Task.FromResult(37), "Thirty Seven");
        }
    }


    namespace InitializerTestMessages
    {
        using System.Collections.Generic;


        public interface SimpleRequest
        {
            string Name { get; }
        }


        public interface SimpleResponse
        {
            string Name { get; }
            string Value { get; }
        }


        public interface ComplexRequest
        {
            string Name { get; }
            int IntValue { get; }
            int? NullableIntValue { get; }
        }


        public interface ComplexResponse
        {
            string Name { get; }
            int? IntValue { get; }
            int NullableIntValue { get; }
        }


        public enum Status
        {
            Unknown = 0,
            Started = 1,
            Stopped = 12
        }


        public interface SuperComplexRequest
        {
            Guid CorrelationId { get; }
            Guid Id { get; }
            string StringId { get; }
            string StringValue { get; }
            int IntValue { get; }
            DateTime DateTimeValue { get; }
            long? NullableValue { get; }
            long NotNullableValue { get; }
            decimal? NullableDecimalValue { get; }
            string[] Numbers { get; }
            string[] Names { get; }
            string[] Amounts { get; }
            ExceptionInfo Exception { get; }
            SubValue SubValue { get; }
            SubValue[] SubValues { get; }
            DateTime Timestamp { get; }
            string Amount { get; }
            long AsyncValue { get; }
            Status EngineStatus { get; }
            Status NumberStatus { get; }
            Status StringStatus { get; }
            Uri ServiceAddress { get; }
            Uri OtherAddress { get; }
            string StringAddress { get; }
            List<string> StringList { get; }
            AProperty NewProperty { get; }

            IDictionary<string, string> Strings { get; }
            IDictionary<string, SubValue> StringSubValues { get; }
            IDictionary<int, long> IntToStrings { get; }
        }


        public interface SuperComplexResponse
        {
            Guid CorrelationId { get; }
            Guid Id { get; }
            Guid StringId { get; }
            string StringValue { get; }
            int IntValue { get; }
            DateTime DateTimeValue { get; }
            long? NullableValue { get; }
            long NotNullableValue { get; }
            decimal? NullableDecimalValue { get; }
            string[] Numbers { get; }
            string[] Names { get; }
            IList<decimal> Amounts { get; }
            ExceptionInfo Exception { get; }
            SubValue SubValue { get; }
            SubValue[] SubValues { get; }
            DateTime? Timestamp { get; }
            decimal Amount { get; }
            long AsyncValue { get; }
            Status EngineStatus { get; }
            Status NumberStatus { get; }
            Status StringStatus { get; }
            Uri ServiceAddress { get; }
            Uri OtherAddress { get; }
            string StringAddress { get; }
            IList<string> StringList { get; }
            AProperty NewProperty { get; }

            IDictionary<string, string> Strings { get; }
            IDictionary<string, SubValue> StringSubValues { get; }
            IDictionary<long, string> IntToStrings { get; }
        }


        public interface SubValue
        {
            string Text { get; }
        }


        public interface AProperty
        {
            string NewProperty { get; }
        }


        class BaseProperty
        {
            public string NewProperty { get; set; }
        }


        class SubProperty :
            BaseProperty
        {
            public new string NewProperty { get; set; }
        }
    }
}
