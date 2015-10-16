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
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using Util;


    [TestFixture]
    public class Failing_to_connect_to_rabbitmq :
        AsyncTestFixture
    {
        [Test]
        [ExpectedException(typeof(RabbitMqConnectionException))]
        public async Task Should_fault_nicely()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://unknownhost:32787"), h =>
                {
                    h.Username("whocares");
                    h.Password("Ohcrud");
                });
            });

            using (var handle = busControl.Start())
            {
                Console.WriteLine("Waiting for connection...");

                TaskUtil.Await(() => handle.Ready);
            }
        }

        [Test]
        [ExpectedException(typeof(RabbitMqConnectionException))]
        public async Task Should_fault_when_credentials_are_bad()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guessed");
                });
            });

            using (var handle = busControl.Start())
            {
                Console.WriteLine("Waiting for connection...");

                TaskUtil.Await(() => handle.Ready);
            }
        }
    }
}