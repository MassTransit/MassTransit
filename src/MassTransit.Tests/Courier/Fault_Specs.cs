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
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_an_activity_faults :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_capture_a_thrown_exception()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipFaulted>> handled = ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            ActivityTestContext faultActivity = GetActivityContext<NastyFaultyActivity>();

            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await handled;
        }

        [Test]
        public async Task Should_compensate_both_activities()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipFaulted>> handled = ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext faultActivity = GetActivityContext<NastyFaultyActivity>();

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello Again!",
            });
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello Again!",
            });
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await handled;
        }

        [Test]
        public async Task Should_handle_the_failed_to_compensate_event()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipActivityCompensationFailed>> handledCompensationFailure =
                ConnectPublishHandler<RoutingSlipActivityCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);
            Task<ConsumeContext<RoutingSlipCompensationFailed>> handledRoutingSlipFailure =
                ConnectPublishHandler<RoutingSlipCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext faultyCompensateActivity = GetActivityContext<FaultyCompensateActivity>();
            ActivityTestContext faultActivity = GetActivityContext<FaultyActivity>();

            builder.AddVariable("Value", "Hello");
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
            builder.AddActivity(faultyCompensateActivity.Name, faultyCompensateActivity.ExecuteUri);
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await handledRoutingSlipFailure;

            await handledCompensationFailure;
        }

        [Test]
        public async Task Should_handle_the_failed_to_compensate_event_via_subscription()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            Task<ConsumeContext<RoutingSlipActivityCompensationFailed>> handledCompensationFailure =
                SubscribeHandler<RoutingSlipActivityCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);
            Task<ConsumeContext<RoutingSlipCompensationFailed>> handledRoutingSlipFailure =
                SubscribeHandler<RoutingSlipCompensationFailed>(x => x.Message.TrackingNumber == builder.TrackingNumber);

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext faultyCompensateActivity = GetActivityContext<FaultyCompensateActivity>();
            ActivityTestContext faultActivity = GetActivityContext<FaultyActivity>();

            builder.AddVariable("Value", "Hello");
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);
            builder.AddActivity(faultyCompensateActivity.Name, faultyCompensateActivity.ExecuteUri);
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri);

            builder.AddSubscription(
                Bus.Address,
                RoutingSlipEvents.CompensationFailed,
                RoutingSlipEventContents.All);

            builder.AddSubscription(
                Bus.Address,
                RoutingSlipEvents.ActivityCompensationFailed,
                RoutingSlipEventContents.All);

            await Bus.Execute(builder.Build());

            await handledRoutingSlipFailure;

            await handledCompensationFailure;
        }

        [Test]
        public async Task Should_publish_the_faulted_routing_slip_event()
        {
            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondTestActivity = GetActivityContext<SecondTestActivity>();
            ActivityTestContext faultActivity = GetActivityContext<FaultyActivity>();

            Task<ConsumeContext<RoutingSlipFaulted>> handled = ConnectPublishHandler<RoutingSlipFaulted>(x => x.Message.TrackingNumber == builder.TrackingNumber);
            Task<ConsumeContext<RoutingSlipActivityCompensated>> compensated = ConnectPublishHandler<RoutingSlipActivityCompensated>(
                context => context.Message.ActivityName.Equals(testActivity.Name));

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
            });
            builder.AddActivity(secondTestActivity.Name, secondTestActivity.ExecuteUri, new
            {
                Value = "Hello Again!",
            });
            builder.AddActivity(faultActivity.Name, faultActivity.ExecuteUri, new
            {
            });

            await Bus.Execute(builder.Build());

            await handled;

            await compensated;
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());
            AddActivityContext<SecondTestActivity, TestArguments, TestLog>(() => new SecondTestActivity());
            AddActivityContext<FaultyCompensateActivity, TestArguments, TestLog>(() => new FaultyCompensateActivity());
            AddActivityContext<FaultyActivity, FaultyArguments, FaultyLog>(() => new FaultyActivity());
            AddActivityContext<NastyFaultyActivity, FaultyArguments, FaultyLog>(() => new NastyFaultyActivity());
        }
    }
}