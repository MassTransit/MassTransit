namespace MassTransit.Tests.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class Storing_an_object_graph_as_a_variable_or_argument :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_work_for_activity_arguments()
        {
            _intValue = 27;
            _stringValue = "Hello, World.";
            _decimalValue = 123.45m;

            Task<ConsumeContext<RoutingSlipCompleted>> completed = await ConnectPublishHandler<RoutingSlipCompleted>();
            Task<ConsumeContext<RoutingSlipFaulted>> faulted = await ConnectPublishHandler<RoutingSlipFaulted>();

            var testActivity = GetActivityContext<ObjectGraphTestActivity>();
            var testActivity2 = GetActivityContext<TestActivity>();

            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var dictionary = new Dictionary<string, object>
            {
                { "Outer", new OuterObjectImpl(_intValue, _stringValue, _decimalValue) },
                { "Names", new[] { "Albert", "Chris" } },
                { "ArgumentsDictionary", _argumentsDictionary }
            };
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, dictionary);
            builder.AddActivity(testActivity2.Name, testActivity2.ExecuteUri, new { Value = "Howdy!" });

            builder.AddVariable("ArgumentsDictionary", new Dictionary<string, string>
            {
                { "good_jpath_key", "val3" },
                { "bad jpath key", "val4" }
            });

            await Bus.Execute(builder.Build());

            await Task.WhenAny(completed, faulted);

            if (faulted.Status == TaskStatus.RanToCompletion)
            {
                Assert.Fail(
                    $"Failed due to exception {(faulted.Result.Message.ActivityExceptions.Any() ? faulted.Result.Message.ActivityExceptions.First().ExceptionInfo.Message : "VisitUnknownFilter")}");
            }

            Assert.That(completed.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        int _intValue;
        string _stringValue;
        decimal _decimalValue;

        readonly IDictionary<string, string> _argumentsDictionary = new Dictionary<string, string>
        {
            { "good_jpath_key", "val1" },
            { "bad jpath key", "val2" }
        };

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<ObjectGraphTestActivity, ObjectGraphActivityArguments, TestLog>(
                () => new ObjectGraphTestActivity(_intValue, _stringValue, _decimalValue, new[] { "Albert", "Chris" }, _argumentsDictionary));
            AddActivityContext<TestActivity, TestArguments, TestLog>(
                () => new TestActivity());
        }
    }


    [TestFixture]
    public class Nullable_value_types_in_routing_slip_arguments :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_not_lose_their_default_value()
        {
            Task<ConsumeContext<RoutingSlipCompleted>> completed = await ConnectPublishHandler<RoutingSlipCompleted>();
            Task<ConsumeContext<RoutingSlipFaulted>> faulted = await ConnectPublishHandler<RoutingSlipFaulted>();

            await InputQueueSendEndpoint.Send(new Message { Payload = new List<SomeClass> { new SomeClass { Enumeration = AnEnumeration.DefaultValue } } });

            await Task.WhenAny(completed, faulted);

            if (faulted.Status == TaskStatus.RanToCompletion)
            {
                Assert.Fail(
                    $"Failed due to exception {(faulted.Result.Message.ActivityExceptions.Any() ? faulted.Result.Message.ActivityExceptions.First().ExceptionInfo.Message : "VisitUnknownFilter")}");
            }

            Assert.That(completed.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureJsonSerializerOptions(j =>
            {
                j.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                return j;
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new AConsumer(GetActivityContext<AnActivity>()));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<AnActivity, Argument>(() => new AnActivity());
        }


        public class AConsumer :
            IConsumer<Message>
        {
            readonly ActivityTestContext _activityTestContext;

            public AConsumer(ActivityTestContext activityTestContext)
            {
                _activityTestContext = activityTestContext;
            }

            public Task Consume(ConsumeContext<Message> context)
            {
                var routingSlip = new RoutingSlipBuilder(Guid.NewGuid());
                var arg = new Argument { Payload = context.Message.Payload };
                routingSlip.AddActivity(_activityTestContext.Name, _activityTestContext.ExecuteUri, arg);
                return context.Execute(routingSlip.Build());
            }
        }


        public class AnActivity : IExecuteActivity<Argument>
        {
            public Task<ExecutionResult> Execute(ExecuteContext<Argument> context)
            {
                AnEnumeration? shouldNotBeNull = context.Arguments.Payload?.FirstOrDefault()?.Enumeration;

                if (shouldNotBeNull.HasValue)
                    return Task.FromResult(context.Completed());

                throw new ArgumentNullException(nameof(context), "Enumeration was null");
            }
        }


        public class Message
        {
            public List<SomeClass> Payload { get; set; }
        }


        public class Argument
        {
            public List<SomeClass> Payload { get; set; }
        }


        public class SomeClass
        {
            public AnEnumeration? Enumeration { get; set; }
        }


        public enum AnEnumeration
        {
            DefaultValue,
            OtherValue
        }
    }
}
