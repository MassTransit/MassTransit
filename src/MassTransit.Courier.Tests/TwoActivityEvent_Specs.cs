// Copyright 2007-2013 Chris Patterson
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
namespace MassTransit.Courier.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using NUnit.Framework;
    using Testing;


//    [TestFixture]
//    public class Executing_a_routing_slip_with_two_activities :
//        ActivityTestFixture
//    {
//        [Test]
//        public void Should_include_the_activity_log_data()
//        {
//            RoutingSlipActivityCompleted activityCompleted = _firstActivityCompleted.Task.Result;
//
//            Assert.AreEqual("Hello", activityCompleted.Data["OriginalValue"]);
//        }
//
//        [Test]
//        public void Should_include_the_variable_set_by_the_activity()
//        {
//            RoutingSlipCompleted completed = _completed.Task.Result;
//
//            Assert.AreEqual("Hello, World!", completed.Variables["Value"]);
//        }
//
//        [Test]
//        public void Should_include_the_variables_of_the_completed_routing_slip()
//        {
//            RoutingSlipCompleted completed = _completed.Task.Result;
//
//            Assert.AreEqual("Knife", completed.Variables["Variable"]);
//        }
//
//        [Test]
//        public void Should_include_the_variables_with_the_activity_log()
//        {
//            RoutingSlipActivityCompleted activityCompleted = _firstActivityCompleted.Task.Result;
//
//            Assert.AreEqual("Knife", activityCompleted.Variables["Variable"]);
//        }
//
//        [Test]
//        public void Should_receive_the_first_routing_slip_activity_completed_event()
//        {
//            RoutingSlipActivityCompleted activityCompleted = _firstActivityCompleted.Task.Result;
//
//            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
//        }
//
//        [Test]
//        public void Should_receive_the_second_routing_slip_activity_completed_event()
//        {
//            RoutingSlipActivityCompleted activityCompleted = _secondActivityCompleted.Task.Result;
//
//            Assert.AreEqual(_routingSlip.TrackingNumber, activityCompleted.TrackingNumber);
//        }
//
//        [Test]
//        public void Should_receive_the_routing_slip_completed_event()
//        {
//            RoutingSlipCompleted completed = _completed.Task.Result;
//
//            Assert.AreEqual(_routingSlip.TrackingNumber, completed.TrackingNumber);
//        }
//
//        TaskCompletionSource<RoutingSlipCompleted> _completed;
//        TaskCompletionSource<RoutingSlipActivityCompleted> _firstActivityCompleted;
//        TaskCompletionSource<RoutingSlipActivityCompleted> _secondActivityCompleted;
//        RoutingSlip _routingSlip;
//
//        protected override void SetupActivities()
//        {
//            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
//            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
//        }
//
//        [TestFixtureSetUp]
//        public void Setup()
//        {
//            _completed = new TaskCompletionSource<RoutingSlipCompleted>(TestCancellationToken);
//            _firstActivityCompleted = new TaskCompletionSource<RoutingSlipActivityCompleted>(TestCancellationToken);
//            _secondActivityCompleted = new TaskCompletionSource<RoutingSlipActivityCompleted>(TestCancellationToken);
//
//            LocalBus.SubscribeHandler<RoutingSlipCompleted>(x => _completed.SetResult(x));
//            Assert.IsTrue(WaitForSubscription<RoutingSlipCompleted>());
//
//            LocalBus.SubscribeHandler<RoutingSlipActivityCompleted>(x =>
//                {
//                    if (x.ActivityName.Equals("Test"))
//                        _firstActivityCompleted.SetResult(x);
//                    if (x.ActivityName.Equals("SecondTest"))
//                        _secondActivityCompleted.SetResult(x);
//                });
//            Assert.IsTrue(WaitForSubscription<RoutingSlipActivityCompleted>());
//
//            var builder = new RoutingSlipBuilder(Guid.NewGuid());
//
//            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
//                {
//                    Value = "Hello",
//                    NullValue = (string)null,
//                });
//
//            testActivity = GetActivityContext<SecondTestActivity>();
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
//
//            builder.AddVariable("Variable", "Knife");
//            builder.AddVariable("Nothing", null);
//
//            _routingSlip = builder.Build();
//
//            LocalBus.Execute(_routingSlip);
//        }
//    }
}