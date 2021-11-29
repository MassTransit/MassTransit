namespace MassTransit.Tests.Courier
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;
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
                Assert.Fail("Failed due to exception {0}", faulted.Result.Message.ActivityExceptions.Any()
                    ? faulted.Result.Message.ActivityExceptions.First()
                        .ExceptionInfo.Message
                    : "VisitUnknownFilter");
            }

            completed.Status.ShouldBe(TaskStatus.RanToCompletion);
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
}
