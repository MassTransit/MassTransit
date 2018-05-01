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
    public class MixedConsumer_Specs :
        InMemoryTestFixture
    {
        MixedConsumer _consumer;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _consumer = new MixedConsumer(GetTask<A>(), GetTask<B>());

            configurator.Instance(_consumer);
        }


        class MixedConsumer :
        #pragma warning disable 618
            Consumes<A>.All,
        #pragma warning restore 618
            IConsumer<B>
        {
            TaskCompletionSource<A> _receivedA;
            TaskCompletionSource<B> _receivedB;

            public MixedConsumer(TaskCompletionSource<A> getTask, TaskCompletionSource<B> taskCompletionSource)
            {
                _receivedB = taskCompletionSource;
                _receivedA = getTask;
            }

            public TaskCompletionSource<A> ReceivedA
            {
                get { return _receivedA; }
            }

            public TaskCompletionSource<B> ReceivedB
            {
                get { return _receivedB; }
            }

            public void Consume(A message)
            {
                _receivedA.TrySetResult(message);
            }

            public Task Consume(ConsumeContext<B> context)
            {
                _receivedB.TrySetResult(context.Message);

                return TaskUtil.Completed;
            }
        }


        class A
        {
        }


        class B
        {
        }


        [Test]
        public async Task Should_get_both_messages()
        {
            await Bus.Publish(new A());
            await Bus.Publish(new B());

            await _consumer.ReceivedA.Task;

            await _consumer.ReceivedB.Task;
        }
    }
}