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
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Partitioning;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Courier;


    [TestFixture]
    public class When_partitioning_by_activity_arguments :
        InMemoryActivityTestFixture
    {
        [Test]
        public async Task Should_publish_the_completed_event()
        {
            Task<ConsumeContext<RoutingSlipCompleted>> completed = SubscribeHandler<RoutingSlipCompleted>();

            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddSubscription(Bus.Address, RoutingSlipEvents.All);

            var testActivity = GetActivityContext<TestActivity>();
            builder.AddActivity(testActivity.Name, testActivity.ExecuteUri, new
            {
                Value = "Hello"
            });

            await Bus.Execute(builder.Build());

            await completed;
        }

        IPartitioner _partitioner;

        protected override void SetupActivities(BusTestHarness testHarness)
        {
            testHarness.OnConfigureBus += ConfigureBus;

            AddActivityContext<TestActivity, TestArguments, TestLog>(() => new TestActivity(), h => h.UsePartitioner(_partitioner, args => args.Arguments.Value),
                h => h.UsePartitioner(_partitioner, args => args.Log.OriginalValue));
        }

        void ConfigureBus(IBusFactoryConfigurator configurator)
        {
            _partitioner = configurator.CreatePartitioner(8);
        }
    }
}