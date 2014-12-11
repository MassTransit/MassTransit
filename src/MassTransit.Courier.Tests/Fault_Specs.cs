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
//    using System;
//    using System.Diagnostics;
//    using System.Threading;
//    using Contracts;
//    using Magnum.Extensions;
//    using NUnit.Framework;
//    using Testing;
//
//
//    [TestFixture]
//    public class When_an_activity_faults :
//        ActivityTestFixture
//    {
//        [Test]
//        public void Should_run_the_compensation()
//        {
//            var handled = new ManualResetEvent(false);
//
//            LocalBus.SubscribeHandler<RoutingSlipFaulted>(message => { handled.Set(); });
//
//            Assert.IsTrue(WaitForSubscription<RoutingSlipFaulted>());
//
//            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
//            ActivityTestContext secondTestActivity = GetActivityContext<SecondTestActivity>();
//            ActivityTestContext faultActivity = GetActivityContext<FaultyActivity>();
//
//            var builder = new RoutingSlipBuilder(Guid.NewGuid());
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
//                {
//                    Value = "Hello",
//                });
//            builder.AddActivity(secondTestActivity.Name, secondTestActivity.ExecuteUri, new
//                {
//                    Value = "Hello Again!",
//                });
//            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new
//                {
//                });
//
//            LocalBus.Execute(builder.Build());
//
//            Assert.IsTrue(handled.WaitOne(Debugger.IsAttached ? 5.Minutes() : 30.Seconds()));
//        }
//
//        [Test]
//        public void Should_capture_a_thrown_exception()
//        {
//            var handled = new ManualResetEvent(false);
//
//            LocalBus.SubscribeHandler<RoutingSlipFaulted>(message => handled.Set());
//
//            Assert.IsTrue(WaitForSubscription<RoutingSlipFaulted>());
//
//            ActivityTestContext faultActivity = GetActivityContext<NastyFaultyActivity>();
//
//            var builder = new RoutingSlipBuilder(Guid.NewGuid());
//            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);
//
//            LocalBus.Execute(builder.Build());
//
//            Assert.IsTrue(handled.WaitOne(Debugger.IsAttached ? 5.Minutes() : 30.Seconds()));
//        }
//
//        [Test]
//        public void Should_compensate_both_activities()
//        {
//            var handled = new ManualResetEvent(false);
//
//            LocalBus.SubscribeHandler<RoutingSlipFaulted>(message => handled.Set());
//
//            Assert.IsTrue(WaitForSubscription<RoutingSlipFaulted>());
//
//            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
//            ActivityTestContext faultActivity = GetActivityContext<NastyFaultyActivity>();
//
//            var builder = new RoutingSlipBuilder(Guid.NewGuid());
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
//            {
//                Value = "Hello Again!",
//            });
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
//            {
//                Value = "Hello Again!",
//            });
//            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);
//
//            LocalBus.Execute(builder.Build());
//
//            Assert.IsTrue(handled.WaitOne(Debugger.IsAttached ? 5.Minutes() : 30.Seconds()));
//        }
//
//        [Test]
//        public void Should_handle_the_failed_to_compensate_event()
//        {
//            var handledCompensationFailure = new ManualResetEvent(false);
//            var handledRoutingSlipFailure = new ManualResetEvent(false);
//
//            LocalBus.SubscribeHandler<RoutingSlipActivityCompensationFailed>(message => handledCompensationFailure.Set());
//            LocalBus.SubscribeHandler<RoutingSlipCompensationFailed>(message => handledRoutingSlipFailure.Set());
//
//            Assert.IsTrue(WaitForSubscription<RoutingSlipCompensationFailed>());
//            Assert.IsTrue(WaitForSubscription<RoutingSlipActivityCompensationFailed>());
//
//            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
//            ActivityTestContext faultyCompensateActivity = GetActivityContext<FaultyCompensateActivity>();
//            ActivityTestContext faultActivity = GetActivityContext<FaultyActivity>();
//
//            var builder = new RoutingSlipBuilder(Guid.NewGuid());
//            builder.AddVariable("Value", "Hello");
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
//            builder.AddActivity(faultyCompensateActivity.Name, faultyCompensateActivity.ExecuteUri);
//            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);
//
//            LocalBus.Execute(builder.Build());
//
//            Assert.IsTrue(handledRoutingSlipFailure.WaitOne(Debugger.IsAttached ? 5.Minutes() : 30.Seconds()));
//            Assert.IsTrue(handledCompensationFailure.WaitOne(Debugger.IsAttached ? 5.Minutes() : 30.Seconds()));
//        }
//
//        protected override void SetupActivities()
//        {
//            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
//            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
//            AddActivityContext<FaultyCompensateActivity, TestArguments, TestLog>(() => new FaultyCompensateActivity());
//            AddActivityContext<FaultyActivity, FaultyArguments, FaultyLog>(() => new FaultyActivity());
//            AddActivityContext<NastyFaultyActivity, FaultyArguments, FaultyLog>(() => new NastyFaultyActivity());
//        }
//    }
}