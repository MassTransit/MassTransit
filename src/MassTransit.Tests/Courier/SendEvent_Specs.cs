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
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class Send_an_event_at_the_end_of_the_routing_slip :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_publish_the_completed_event()
        {
            var startTime = DateTime.UtcNow;

            Task<ConsumeContext<RoutingSlipCompleted>> completed = SubscribeHandler<RoutingSlipCompleted>();
            var myCompleted = SubscribeHandler<MyRoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);
            await builder.AddSubscription(Bus.Address, RoutingSlipEvents.Completed, x => x.Send<MyRoutingSlipCompleted>(new
            {
                builder.TrackingNumber,
                SomeValue = "Hello"
            }));

            var testActivity = GetActivityContext<TestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello"
            });

            await Bus.Execute(builder.Build());

            await completed;

            var context = await myCompleted;

            Assert.That(context.Message.Timestamp, Is.GreaterThanOrEqualTo(startTime));

            Console.WriteLine(GetBodyString(context.ReceiveContext));
        }

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity(), h =>
            {
                h.UseExecute(context => Console.WriteLine(GetBodyString(context.ReceiveContext)));
            });
        }

        string GetBodyString(ReceiveContext context)
        {
            return Encoding.UTF8.GetString(context.GetBody());
        }


        public interface MyRoutingSlipCompleted
        {
            Guid TrackingNumber { get; }

            string SomeValue { get; }

            DateTime Timestamp { get; }
        }
    }
}
