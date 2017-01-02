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
    public class When_an_activity_uses_an_address :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_publish_the_completed_event()
        {
            Task<ConsumeContext<RoutingSlipCompleted>> completed = SubscribeHandler<RoutingSlipCompleted>();
            Task<ConsumeContext<RoutingSlipActivityCompleted>> activity = SubscribeHandler<RoutingSlipActivityCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            ActivityTestContext testActivity = GetActivityContext<AddressActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri);

            builder.SetVariables(new
            {
                Address = new Uri("http://google.com/"),
            });

            await Bus.Execute(builder.Build());

            await completed;

            var consumeContext = await activity;

            Assert.AreEqual(new Uri("http://google.com/"), consumeContext.Message.GetResult<string>("UsedAddress"));
        }

        [Test]
        public async Task Should_compensate_with_the_log()
        {
            Task<ConsumeContext<RoutingSlipFaulted>> faulted = SubscribeHandler<RoutingSlipFaulted>();
            Task<ConsumeContext<RoutingSlipActivityCompleted>> activity = SubscribeHandler<RoutingSlipActivityCompleted>();
            Task<ConsumeContext<RoutingSlipActivityCompensated>> activityCompensated = SubscribeHandler<RoutingSlipActivityCompensated>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            ActivityTestContext testActivity = GetActivityContext<AddressActivity>();
            ActivityTestContext faultyActivity = GetActivityContext<FaultyActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Address = new Uri("http://google.com/"),
            });
            builder.AddActivity(faultyActivity.Name, faultyActivity.ExecuteUri);

            await Bus.Execute(builder.Build());

            await faulted;

            var consumeContext = await activity;

            Assert.AreEqual(new Uri("http://google.com/"), consumeContext.Message.GetResult<string>("UsedAddress"));

            var context = await activityCompensated;

            Assert.AreEqual(new Uri("http://google.com/"), context.Message.GetResult<string>("UsedAddress"));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<AddressActivity, AddressArguments, AddressLog>(() => new AddressActivity());
            AddActivityContext<FaultyActivity, FaultyArguments>(() => new FaultyActivity());
        }
    }
}