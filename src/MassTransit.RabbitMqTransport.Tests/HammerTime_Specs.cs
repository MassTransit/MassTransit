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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture, Explicit]
    public class Pounding_the_crap_out_of_the_send_endpoint :
        RabbitMqTestFixture
    {
        TaskCompletionSource<int> _completed;

        public Pounding_the_crap_out_of_the_send_endpoint()
        {
            TestTimeout = TimeSpan.FromSeconds(180);
        }

        protected override void ConfigureInputQueueEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _completed = GetTask<int>();
            int count = 0;

            configurator.Handler<PingMessage>(async context =>
            {
                if (Interlocked.Increment(ref count) == 100000)
                    _completed.TrySetResult(count);
            });
        }

        [Test]
        public async void Should_end_well()
        {
            Parallel.For(0, 100, i =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    var ping = new PingMessage();
                    Bus.Publish(ping);
                }
            });

            await _completed.Task;
        }
    }
}