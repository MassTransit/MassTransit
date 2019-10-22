// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using GreenPipes;
    using GreenPipes.Introspection;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;
    using Util;


    [TestFixture]
    public class Executing_a_routing_slip_with_two_activities :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Should_include_the_activity_log_data()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual("Hello", activityCompleted.GetResult<string>("OriginalValue"));
        }

        [Test]
        public async Task Should_include_the_variable_set_by_the_activity()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

            Assert.AreEqual("Hello, World!", completed.GetVariable<string>("Value"));
        }

        [Test]
        public async Task Should_include_the_variables_of_the_completed_routing_slip()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

            Assert.AreEqual("Knife", completed.Variables["Variable"]);
        }

        [Test]
        public async Task Should_include_the_variables_with_the_activity_log()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual("Knife", activityCompleted.Variables["Variable"]);
        }

        [Test]
        public async Task Should_receive_the_first_routing_slip_activity_completed_event()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, completed.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_second_routing_slip_activity_completed_event()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _secondActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        [Test, Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
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

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();

            _firstActivityCompleted = Handled<RoutingSlipActivityCompleted>(configurator, context => context.Message.ActivityName.Equals(testActivity.Name)
                && context.Message.TrackingNumber == _routingSlip.TrackingNumber);
            _secondActivityCompleted = Handled<RoutingSlipActivityCompleted>(configurator, context => context.Message.ActivityName.Equals(secondActivity.Name)
                && context.Message.TrackingNumber == _routingSlip.TrackingNumber);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null,
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);

            _routingSlip = builder.Build();

            await Bus.Execute(_routingSlip);
        }
    }


    [TestFixture]
    public class Executing_with_no_observers :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

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

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null,
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);

            _routingSlip = builder.Build();

            await Bus.Execute(_routingSlip);

            Console.WriteLine("Routing slip executed");
        }
    }


    [TestFixture]
    public class Executing_many_activities_in_a_row :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            int completed = await _allDone.Task;
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
                int count = Interlocked.Increment(ref _count);
                if (count == _limit)
                    _allDone.TrySetResult(count);
            });
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _limit = 100;

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();

            for (int i = 0; i < _limit; i++)
            {
                var builder = new RoutingSlipBuilder(Guid.NewGuid());
                builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
                builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);
                builder.AddVariable("Value", "Hello");

                RoutingSlip routingSlip = builder.Build();

                await Bus.Execute(routingSlip);

                _sentRoutingSlips.Add(routingSlip.TrackingNumber);
            }
        }
    }


    [TestFixture]
    public class Executing_many_activities_in_a_row_with_a_fault_one :
        RabbitMqActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            int completed = await _allDone.Task;
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
                int count = Interlocked.Increment(ref _count);
                if (count == _limit)
                    _allDone.TrySetResult(count);
            });
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _limit = 1;

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();

            for (int i = 0; i < _limit; i++)
            {
                var builder = new RoutingSlipBuilder(Guid.NewGuid());
                builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
                builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

                RoutingSlip routingSlip = builder.Build();

                await Bus.Execute(routingSlip);

                _sentRoutingSlips.Add(routingSlip.TrackingNumber);
            }
        }
    }
}
