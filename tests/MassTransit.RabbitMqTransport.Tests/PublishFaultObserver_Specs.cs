// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture, Category("SlowAF")]
    public class PublishFaultObserver_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_the_observer_fault()
        {
            Bus.ConnectPublishObserver(new PublishObserver());

            for (var i = 0; i < 30; i++)
            {
                try
                {
                    await Task.Delay(1000);

                    await Bus.Publish(new TestMessage {Index = i});
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Publish {i} faulted: {ex.Message}");
                }
            }
        }


        class PublishObserver :
            IPublishObserver
        {
            public Task PostPublish<T>(PublishContext<T> context) where T : class
            {
                return Console.Out.WriteLineAsync($"PostPublish: {((Test)context.Message).Index}");
            }

            public Task PrePublish<T>(PublishContext<T> context) where T : class
            {
                return Console.Out.WriteLineAsync($"PrePublish: {((Test)context.Message).Index}");
            }

            public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
            {
                return Console.Out.WriteLineAsync($"PublishFault: {((Test)context.Message).Index}");
            }
        }


        public interface Test
        {
            int Index { get; }
        }


        public class TestMessage :
            Test
        {
            public int Index { get; set; }
        }
    }
}