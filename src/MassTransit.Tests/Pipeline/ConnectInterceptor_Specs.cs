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
namespace MassTransit.Tests.Pipeline
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using NUnit.Framework;


    [TestFixture]
    public class Connecting_an_interceptor :
        AsyncTestFixture
    {
        [Test]
        public async void Should_invoke_pre()
        {
            IInboundPipe filter = new InboundMessageFilter();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectHandler<MessageA>(async context => received.TrySetResult(context.Message));

            var interceptor = GetMessageInterceptor<MessageA>();
            filter.Connect(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PreDispatched;
        }

        [Test]
        public async void Should_invoke_post()
        {
            IInboundPipe filter = new InboundMessageFilter();

            TaskCompletionSource<MessageA> received = GetTask<MessageA>();

            filter.ConnectHandler<MessageA>(async context => received.TrySetResult(context.Message));

            var interceptor = GetMessageInterceptor<MessageA>();
            filter.Connect(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            await filter.Send(consumeContext);

            await received.Task;

            await interceptor.PostDispatched;
        }

        [Test]
        public void Should_invoke_faulted()
        {
            IInboundPipe filter = new InboundMessageFilter();

            filter.ConnectHandler<MessageA>(async context => { throw new InvalidOperationException("This is a test"); });

            var interceptor = GetMessageInterceptor<MessageA>();
            filter.Connect(interceptor);

            ConsumeContext consumeContext = GetConsumeContext(new MessageA());

            Assert.Throws<AggregateException>(async () => await filter.Send(consumeContext));

            var exception = Assert.Throws<AggregateException>(async () => await interceptor.DispatchedFaulted);

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }
    }
}