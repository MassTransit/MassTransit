// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class Executing_a_routing_slip_with_a_single_activity :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_log()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual("Hello", context.Message.GetResult<string>("OriginalValue"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_variable()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual("Knife", context.Message.GetVariable<string>("Variable"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_timestamps()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;
            ConsumeContext<RoutingSlipCompleted> completeContext = await _completed;

            Assert.AreEqual(completeContext.Message.Timestamp, context.Message.Timestamp + context.Message.Duration);

            Console.WriteLine("Duration: {0}", context.Message.Duration);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_variable()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual("Knife", context.Message.GetVariable<string>("Variable"));
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;

        [OneTimeSetUp]
        public async Task Should_publish_the_completed_event()
        {
            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<TestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello"
            });

            builder.AddVariable("Variable", "Knife");

            await Bus.Execute(builder.Build());

            await _completed;
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
        }
    }


    [TestFixture]
    public class Executing_a_routing_slip_with_a_single_activity_harness
    {
        [Test]
        public async Task Should_receive_the_routing_slip_activity_completed_event()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_log()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual("Hello", context.Message.GetResult<string>("OriginalValue"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_activity_variable()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;

            Assert.AreEqual("Knife", context.Message.GetVariable<string>("Variable"));
        }

        [Test]
        public async Task Should_receive_the_routing_slip_completed_event()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual(_trackingNumber, context.Message.TrackingNumber);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_timestamps()
        {
            ConsumeContext<RoutingSlipActivityCompleted> context = await _activityCompleted;
            ConsumeContext<RoutingSlipCompleted> completeContext = await _completed;

            Assert.AreEqual(completeContext.Message.Timestamp, context.Message.Timestamp + context.Message.Duration);

            Console.WriteLine("Duration: {0}", context.Message.Duration);
        }

        [Test]
        public async Task Should_receive_the_routing_slip_variable()
        {
            ConsumeContext<RoutingSlipCompleted> context = await _completed;

            Assert.AreEqual("Knife", context.Message.GetVariable<string>("Variable"));
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _activityCompleted;
        Guid _trackingNumber;
        InMemoryTestHarness _harness;
        ActivityTestHarness<TestActivity, TestArguments, TestLog> _activity;

        [OneTimeSetUp]
        public async Task Should_publish_the_completed_event()
        {
            _harness = new InMemoryTestHarness();

            _activity = _harness.Activity<TestActivity, TestArguments, TestLog>();

            await _harness.Start();

            _completed = _harness.SubscribeHandler<RoutingSlipCompleted>();
            _activityCompleted = _harness.SubscribeHandler<RoutingSlipActivityCompleted>();

            _trackingNumber = NewId.NextGuid();
            var builder = new RoutingSlipBuilder(_trackingNumber);
            builder.AddSubscription(_harness.BusAddress, RoutingSlipEvents.All);

            builder.AddActivity(_activity.Name, _activity.ExecuteAddress, new
            {
                Value = "Hello"
            });

            builder.AddVariable("Variable", "Knife");

            await _harness.Bus.Execute(builder.Build());

            await _completed;
        }

        [OneTimeTearDown]
        public async Task Done()
        {
            await _harness.Stop();
        }
    }
}