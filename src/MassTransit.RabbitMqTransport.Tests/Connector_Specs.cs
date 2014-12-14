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
namespace MassTransit.RabbitMqTransport.Tests
{
    namespace Connector_Specs
    {
        using System;
        using Configuration;
        using NUnit.Framework;
        using Pipeline;
        using Policies;
        using RabbitMQ.Client.Exceptions;
        using TestFramework;


        [TestFixture]
        public class When_a_server_exists :
            AsyncTestFixture
        {
            [Test]
            public async void Should_connect()
            {
                var hostUri = new Uri("rabbitmq://localhost/");
                RabbitMqHostSettings hostSettings = hostUri.GetHostSettings();

                var testPipe = new TestConnectionPipe(TestCancellationToken);

                IRabbitMqConnector connector = new RabbitMqConnector(hostSettings, Retry.None);
                await connector.Connect(testPipe, TestCancellationToken);

                await testPipe.Called;
            }
        }


        [TestFixture]
        public class When_a_server_does_not_exist :
            AsyncTestFixture
        {
            [Test]
            public void Should_connect()
            {
                var hostUri = new Uri("rabbitmq://rocalhost/");
                RabbitMqHostSettings hostSettings = hostUri.GetHostSettings();

                var testPipe = new TestConnectionPipe(TestCancellationToken);

                IRabbitMqConnector connector = new RabbitMqConnector(hostSettings, Retry.Intervals(100, 100));

                var ex = Assert.Throws<RabbitMqConnectionException>(async () =>
                {
                    await connector.Connect(testPipe, TestCancellationToken);
                });

                Assert.IsInstanceOf<BrokerUnreachableException>(ex.InnerException);
            }

            [Test]
            public void Should_throw_connection_exception()
            {
                var hostUri = new Uri("rabbitmq://rocalhost/");
                RabbitMqHostSettings hostSettings = hostUri.GetHostSettings();

                var testPipe = new TestConnectionPipe(TestCancellationToken);

                IRabbitMqConnector connector = new RabbitMqConnector(hostSettings, Retry.None);

                var ex = Assert.Throws<RabbitMqConnectionException>(async () =>
                {
                    await connector.Connect(testPipe, TestCancellationToken);
                });

                Assert.IsInstanceOf<BrokerUnreachableException>(ex.InnerException);
            }
        }
    }
}