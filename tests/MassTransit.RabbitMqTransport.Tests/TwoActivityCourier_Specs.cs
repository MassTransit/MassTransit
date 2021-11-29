namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework.Courier;


    [TestFixture]
    public class Executing_a_routing_slip_with_two_activities :
        RabbitMqActivityTestFixture
    {
        [Test]
        [Order(1)]
        public async Task A_routing_slip()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var testActivity = GetActivityContext<TestActivity>();
            var secondActivity = GetActivityContext<SecondTestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);

            _routingSlip = builder.Build();

            await Bus.Execute(_routingSlip);
        }

        [Test]
        [Order(2)]
        public async Task Should_include_the_activity_log_data()
        {
            var activityCompleted = (await _firstActivityCompleted);

            Assert.AreEqual("Hello", activityCompleted.GetResult<string>("OriginalValue"));
        }

        [Test]
        [Order(3)]
        public async Task Should_include_the_variable_set_by_the_activity()
        {
            var completed = await _completed;

            Assert.AreEqual("Hello, World!", completed.GetVariable<string>("Value"));
        }

        [Test]
        [Order(3)]
        public async Task Should_include_the_variables_of_the_completed_routing_slip()
        {
            var completed = (await _completed).Message;

            Assert.AreEqual("Knife", completed.Variables["Variable"]);
        }

        [Test]
        [Order(2)]
        public async Task Should_include_the_variables_with_the_activity_log()
        {
            var activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual("Knife", activityCompleted.Variables["Variable"]);
        }

        [Test]
        [Order(2)]
        public async Task Should_receive_the_first_routing_slip_activity_completed_event()
        {
            var activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        [Test]
        [Order(3)]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            var completed = (await _completed).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, completed.TrackingNumber);
        }

        [Test]
        [Order(2)]
        public async Task Should_receive_the_second_routing_slip_activity_completed_event()
        {
            var activityCompleted = (await _secondActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        [Test]
        [Order(9)]
        public async Task Show_timeline()
        {
            var completed = (await _completed).Message;

            await RabbitMqTestHarness.OutputTimeline(TestContext.Out, x => x.Now().IncludeAddress());
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _firstActivityCompleted;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _secondActivityCompleted;
        RoutingSlip _routingSlip;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _completed = Handled<RoutingSlipCompleted>(configurator, x => x.Message.TrackingNumber == _routingSlip.TrackingNumber);

            var testActivity = GetActivityContext<TestActivity>();
            var secondActivity = GetActivityContext<SecondTestActivity>();

            _firstActivityCompleted = Handled<RoutingSlipActivityCompleted>(configurator, context => context.Message.ActivityName.Equals(testActivity.Name)
                && context.Message.TrackingNumber == _routingSlip.TrackingNumber);
            _secondActivityCompleted = Handled<RoutingSlipActivityCompleted>(configurator, context => context.Message.ActivityName.Equals(secondActivity.Name)
                && context.Message.TrackingNumber == _routingSlip.TrackingNumber);
        }
    }


    [TestFixture]
    public class Executing_with_no_observers :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Setup()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            var testActivity = GetActivityContext<TestActivity>();
            var secondActivity = GetActivityContext<SecondTestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);

            _routingSlip = builder.Build();

            await Bus.Execute(_routingSlip);

            var completed = (await _completed).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, completed.TrackingNumber);
        }

        public Executing_with_no_observers()
        {
            TestTimeout = TimeSpan.FromSeconds(60);
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        RoutingSlip _routingSlip;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _completed = Handled<RoutingSlipCompleted>(configurator);
        }
    }


    [Category("Flaky")]
    [TestFixture]
    public class Executing_many_activities_in_a_row :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Setup()
        {
            _limit = 10;

            var testActivity = GetActivityContext<TestActivity>();
            var secondActivity = GetActivityContext<SecondTestActivity>();

            for (var i = 0; i < _limit; i++)
            {
                var builder = new RoutingSlipBuilder(Guid.NewGuid());
                builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
                builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);
                builder.AddVariable("Value", "Hello");

                var routingSlip = builder.Build();

                await Bus.Execute(routingSlip);

                _sentRoutingSlips.Add(routingSlip.TrackingNumber);
            }

            var completed = await _allDone.Task;
        }

        public Executing_many_activities_in_a_row()
        {
            _completedRoutingSlips = new ConcurrentBag<Guid>();
            _sentRoutingSlips = new HashSet<Guid>();

            TestTimeout = TimeSpan.FromMinutes(2);
        }

        readonly ConcurrentBag<Guid> _completedRoutingSlips;
        readonly HashSet<Guid> _sentRoutingSlips;
        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        TaskCompletionSource<int> _allDone;
        int _count;
        int _limit;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _allDone = GetTask<int>();

            _completed = Handler<RoutingSlipCompleted>(configurator, async context =>
            {
                _completedRoutingSlips.Add(context.Message.TrackingNumber);
                var count = Interlocked.Increment(ref _count);
                if (count == _limit)
                    _allDone.TrySetResult(count);
            });
        }
    }


    [Category("Flaky")]
    [TestFixture]
    public class Executing_many_activities_in_a_row_with_a_fault_one :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            _limit = 1;

            var testActivity = GetActivityContext<TestActivity>();
            var secondActivity = GetActivityContext<SecondTestActivity>();

            for (var i = 0; i < _limit; i++)
            {
                var builder = new RoutingSlipBuilder(Guid.NewGuid());
                builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
                builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

                var routingSlip = builder.Build();

                await Bus.Execute(routingSlip);

                _sentRoutingSlips.Add(routingSlip.TrackingNumber);
            }

            var completed = await _allDone.Task;
        }

        public Executing_many_activities_in_a_row_with_a_fault_one()
        {
            _completedRoutingSlips = new ConcurrentBag<Guid>();
            _sentRoutingSlips = new HashSet<Guid>();

            TestTimeout = TimeSpan.FromMinutes(2);
        }

        readonly ConcurrentBag<Guid> _completedRoutingSlips;
        readonly HashSet<Guid> _sentRoutingSlips;
        TaskCompletionSource<int> _allDone;
        int _count;
        int _limit;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _allDone = GetTask<int>();

            Handler<RoutingSlipFaulted>(configurator, async context =>
            {
                _completedRoutingSlips.Add(context.Message.TrackingNumber);
                var count = Interlocked.Increment(ref _count);
                if (count == _limit)
                    _allDone.TrySetResult(count);
            });
        }
    }
}
