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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RabbitMqTransport.Testing;


    [TestFixture]
    public class Failure_Specs
    {
        [Test, Explicit]
        public async Task Should_properly_fail_on_exclusive_launch()
        {
            var harness1 = new RabbitMqTestHarness();
            harness1.OnConfigureRabbitMqBus += configurator =>
            {
                configurator.OverrideDefaultBusEndpointQueueName("exclusively-yours");
                configurator.Exclusive = true;
            };

            var harness2 = new RabbitMqTestHarness();
            harness2.OnConfigureRabbitMqBus += configurator =>
            {
                configurator.OverrideDefaultBusEndpointQueueName("exclusively-yours");
                configurator.Exclusive = true;
            };

            await harness1.Start();
            try
            {
                await harness2.Start();

                await harness2.Stop();
            }
            catch (RabbitMqConnectionException exception)
            {
            }
            finally
            {
                await Task.Delay(1000);

                await harness1.Stop();
            }
        }
    }
}