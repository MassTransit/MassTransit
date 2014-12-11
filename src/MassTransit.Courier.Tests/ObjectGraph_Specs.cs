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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Testing;


//    [TestFixture]
//    public class Storing_an_object_graph_as_a_variable_or_argument :
//        ActivityTestFixture
//    {
//        [Test]
//        public void Should_work_for_activity_arguments()
//        {
//            _intValue = 27;
//            _stringValue = "Hello, World.";
//            _decimalValue = 123.45m;
//
//            var completed = new TaskCompletionSource<RoutingSlipCompleted>();
//            var faulted = new TaskCompletionSource<RoutingSlipFaulted>();
//
//            LocalBus.SubscribeHandler<RoutingSlipCompleted>(message => completed.TrySetResult(message));
//            LocalBus.SubscribeHandler<RoutingSlipFaulted>(message => faulted.TrySetResult(message));
//
//            Assert.IsTrue(WaitForSubscription<RoutingSlipCompleted>());
//            Assert.IsTrue(WaitForSubscription<RoutingSlipFaulted>());
//
//            var builder = new RoutingSlipBuilder(Guid.NewGuid());
//
//            ActivityTestContext testActivity = GetActivityContext<ObjectGraphTestActivity>();
//            ActivityTestContext testActivity2 = GetActivityContext<TestActivity>();
//
//            var dictionary = new Dictionary<string, object>
//                {
//                    {"Outer", new OuterObjectImpl(_intValue, _stringValue, _decimalValue)},
//                    {"Names", new[] {"Albert", "Chris"}},
//                };
//            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, dictionary);
//            builder.AddActivity(testActivity2.Name, testActivity2.ExecuteUri);
//
//            LocalBus.Execute(builder.Build());
//
//            Assert.AreNotEqual(WaitHandle.WaitTimeout,
//                Task.WaitAny(new Task[] { completed.Task, faulted.Task }, Debugger.IsAttached
//                                                                            ? 5.Minutes()
//                                                                            : 30.Seconds()));
//
//            if (faulted.Task.Status == TaskStatus.RanToCompletion)
//            {
//                Assert.Fail("Failed due to exception {0}", faulted.Task.Result.ActivityExceptions.Any()
//                                                               ? faulted.Task.Result.ActivityExceptions.First()
//                                                                        .ExceptionInfo.Message
//                                                               : "Unknown");
//            }
//
//            Assert.AreEqual(TaskStatus.RanToCompletion, completed.Task.Status, "Did not complete");
//        }
//
//        int _intValue;
//        string _stringValue;
//        decimal _decimalValue;
//
//        protected override void SetupActivities()
//        {
//            AddActivityContext<ObjectGraphTestActivity, ObjectGraphActivityArguments, TestLog>(
//                () => new ObjectGraphTestActivity(_intValue, _stringValue, _decimalValue, new[]{"Albert", "Chris"}));
//            AddActivityContext<TestActivity, TestArguments, TestLog>(
//                () => new TestActivity());
//        }
//    }
}