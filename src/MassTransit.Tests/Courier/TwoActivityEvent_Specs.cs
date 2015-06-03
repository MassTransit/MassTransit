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
namespace MassTransit.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class Executing_a_routing_slip_with_two_activities :
        InMemoryActivityTestFixture
    {
        [Test]
        public async void Should_include_the_activity_log_data()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual("Hello", activityCompleted.GetResult<string>("OriginalValue"));
        }

        [Test]
        public async void Should_include_the_variable_set_by_the_activity()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

            Assert.AreEqual("Hello, World!", completed.GetVariable<string>("Value"));
        }

        [Test]
        public async void Should_include_the_variables_of_the_completed_routing_slip()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

            Assert.AreEqual("Knife", completed.Variables["Variable"]);
        }

        [Test]
        public async void Should_include_the_variables_with_the_activity_log()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual("Knife", activityCompleted.Variables["Variable"]);
        }

        [Test]
        public async void Should_receive_the_first_routing_slip_activity_completed_event()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _firstActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        [Test]
        public async void Should_receive_the_routing_slip_completed_event()
        {
            RoutingSlipCompleted completed = (await _completed).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, completed.TrackingNumber);
        }

        [Test]
        public async void Should_receive_the_second_routing_slip_activity_completed_event()
        {
            RoutingSlipActivityCompleted activityCompleted = (await _secondActivityCompleted).Message;

            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
        }

        Task<ConsumeContext<RoutingSlipCompleted>> _completed;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _firstActivityCompleted;
        Task<ConsumeContext<RoutingSlipActivityCompleted>> _secondActivityCompleted;
        RoutingSlip _routingSlip;

        protected override void SetupActivities()
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();

            _completed = SubscribeHandler<RoutingSlipCompleted>();
            _firstActivityCompleted =
                SubscribeHandler<RoutingSlipActivityCompleted>(context => context.Message.ActivityName.Equals(testActivity.Name));
            _secondActivityCompleted =
                SubscribeHandler<RoutingSlipActivityCompleted>(context => context.Message.ActivityName.Equals(secondActivity.Name));

            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null,
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);

            _routingSlip = builder.Build();

            Bus.Execute(_routingSlip)
                .Wait(TestCancellationToken);
        }
    }
}