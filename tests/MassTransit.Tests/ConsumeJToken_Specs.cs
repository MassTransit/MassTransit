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
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class ConsumeJToken_Specs :
        InMemoryTestFixture
    {
        TaskCompletionSource<JToken> _completed;

        [Test]
        public async Task Should_receive_the_jtoken()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            await _completed.Task;
        }

        [SetUp]
        public void Setup()
        {
            _completed = GetTask<JToken>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<JToken>(async context =>
            {
                await Console.Out.WriteLineAsync($"Received the token! {context.Message}");

                _completed.TrySetResult(context.Message);
            });
        }
    }
}