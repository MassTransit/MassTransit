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
    using GreenPipes;
    using MassTransit.Pipeline;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Connecting_a_handler_to_the_inbound_pipe :
        MessageTestFixture
    {
        [Test]
        public async Task Should_receive_a_test_message()
        {
            IConsumePipe filter = CreateConsumePipe();

            TaskCompletionSource<PingMessage> received = GetTask<PingMessage>();

            ConnectHandle connectHandle = filter.ConnectHandler<PingMessage>(async context =>
                received.TrySetResult(context.Message));

            var consumeContext = new TestConsumeContext<PingMessage>(new PingMessage());

            await filter.Send(consumeContext);

            await received.Task;
        }
    }
}