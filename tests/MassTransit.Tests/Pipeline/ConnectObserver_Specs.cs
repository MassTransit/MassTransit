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
namespace MassTransit.Tests.Pipeline
{
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using MassTransit.Testing.Observers;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Connecting_an_observer :
        MessageTestFixture
    {
        [Test]
        public void Should_invoke_faulted()
        {
            IConsumePipe filter = CreateConsumePipe();

            filter.ConnectHandler<MessageA>(async context =>
            {
                throw new IntentionalTestException("This is a test");
            });

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            Assert.That(async () => await filter.Send(consumeContext), Throws.TypeOf<IntentionalTestException>());

            Assert.That(async () => await interceptor.ConsumeFaulted, Throws.TypeOf<IntentionalTestException>());
        }

        [Test]
        public async Task Should_invoke_post()
        {
            IConsumePipe filter = CreateConsumePipe();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectHandler<MessageA>(async context => received.TrySetResult(context.Message));

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PostConsumed;
        }

        [Test]
        public async Task Should_invoke_post_consumer()
        {
            IConsumePipe filter = CreateConsumePipe();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectConsumer(() => new OneMessageConsumer(received));

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PostConsumed;
        }

        [Test]
        public async Task Should_invoke_pre()
        {
            IConsumePipe filter = CreateConsumePipe();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectHandler<MessageA>(async context => received.TrySetResult(context.Message));

            TestConsumeMessageObserver<MessageA> interceptor = GetConsumeObserver<MessageA>();
            filter.ConnectConsumeMessageObserver(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PreConsumed;
        }
    }
}