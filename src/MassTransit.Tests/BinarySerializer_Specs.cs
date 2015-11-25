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
namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using Util;
    


    [TestFixture]
    public class When_using_the_binary_serializer :
        InMemoryTestFixture
    {
        readonly TaskCompletionSource<Fault<A>> _faultTaskTcs = new TaskCompletionSource<Fault<A>>();

        protected override void ConfigureBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseBinarySerializer();
            base.ConfigureBus(configurator);
        }
        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInputQueueEndpoint(configurator);

            #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

            configurator.Handler<A>(async m =>
            {
                throw new System.Exception("Booom!");
            });

            configurator.Handler<Fault<A>>(async m =>
            {
                _faultTaskTcs.TrySetResult(m.Message);
            });

            #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        }


        [System.Serializable]
        class A
        {
        }



        [Test]
        public async Task Should_be_possibile_to_send_and_consume_faults()
        {
            await Bus.Publish(new A());
            var faultTask = _faultTaskTcs.Task;
            var completedTask = await Task.WhenAny(faultTask, Task.Delay(2000));
            Assert.AreEqual(faultTask, completedTask);
        }
    }
}