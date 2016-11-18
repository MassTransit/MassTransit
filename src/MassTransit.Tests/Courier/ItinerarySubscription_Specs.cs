// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Shouldly;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_an_activity_adds_a_subscription :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_continue_with_the_source_itinerary()
        {
            var trackingNumber = Guid.NewGuid();

            var testActivity = GetActivityContext<TestActivity>();
            var reviseActivity = GetActivityContext<ReviseItineraryActivity>();

            Task<ConsumeContext<RoutingSlipCompleted>> completed =
                SubscribeHandler<RoutingSlipCompleted>(context => (context.Message.TrackingNumber == trackingNumber));

            Task<ConsumeContext<RoutingSlipActivityCompleted>> testActivityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(testActivity.Name));

            Task<ConsumeContext<RoutingSlipActivityCompleted>> reviseActivityCompleted = SubscribeHandler<RoutingSlipActivityCompleted>(
                context => context.Message.TrackingNumber == trackingNumber && context.Message.ActivityName.Equals(reviseActivity.Name));

            var builder = new RoutingSlipBuilder(trackingNumber);
            builder.AddActivity(reviseActivity.Name, reviseActivity.ExecuteUri, new
            {
                Value = "Time to add a new item!"
            });

            await Bus.Execute(builder.Build());

            await completed;
            await reviseActivityCompleted;
            ConsumeContext<RoutingSlipActivityCompleted> testActivityResult = await testActivityCompleted;

            testActivityResult.Message.GetArgument<string>("Value").ShouldBe("Added");

            ConsumeContext<RoutingSlipActivityCompleted> consumeContext = await _handled;

            Assert.That(consumeContext.Message.GetArgument<string>("Value"), Is.EqualTo("Added"));
        }

        Task<ConsumeContext<RoutingSlipActivityCompleted>> _handled;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInputQueueEndpoint(configurator);

            var testActivity = GetActivityContext<TestActivity>();

            _handled = Handled<RoutingSlipActivityCompleted>(configurator, x => x.Message.ActivityName == testActivity.Name);
        }

        protected override void SetupActivities()
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity());

            var testActivity = GetActivityContext<TestActivity>();

            AddActivityContext<ReviseItineraryActivity, TestArguments, TestLog>(() => new ReviseItineraryActivity(x =>
            {
                x.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
                {
                    Value = "Added"
                });
                x.AddSubscription(InputQueueAddress, RoutingSlipEvents.ActivityCompleted | RoutingSlipEvents.Supplemental, RoutingSlipEventContents.All,
                    testActivity.Name);
            }));
        }
    }
}