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
    using GreenPipes;
    using GreenPipes.Introspection;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class Executing_a_routing_slip_with_two_activities :
        InMemoryActivityTestFixture
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

            Console.WriteLine("Duration: {0}", completed.Duration);

            Assert.IsFalse(completed.Variables.ContainsKey("ToBeRemoved"));
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

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ReceiveEndpoint("events", x =>
            {
                _completed = Handled<RoutingSlipCompleted>(x);

                ActivityTestContext testActivity = GetActivityContext<TestActivity>();
                ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();

                _firstActivityCompleted =
                    Handled<RoutingSlipActivityCompleted>(x, context => context.Message.ActivityName.Equals(testActivity.Name));
                _secondActivityCompleted =
                    Handled<RoutingSlipActivityCompleted>(x, context => context.Message.ActivityName.Equals(secondActivity.Name));
            });
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            ActivityTestContext testActivity = GetActivityContext<TestActivity>();
            ActivityTestContext secondActivity = GetActivityContext<SecondTestActivity>();

            var builder = new RoutingSlipBuilder(Guid.NewGuid());

            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello",
                NullValue = (string)null,
            });

            builder.AddActivity(secondActivity.Name, secondActivity.ExecuteUri);

            builder.AddVariable("Variable", "Knife");
            builder.AddVariable("Nothing", null);
            builder.AddVariable("ToBeRemoved", "Existing");

            _routingSlip = builder.Build();

            await Bus.Execute(_routingSlip);
        }
    }
}