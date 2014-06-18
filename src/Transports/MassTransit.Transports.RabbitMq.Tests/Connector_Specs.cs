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
namespace MassTransit.Transports.RabbitMq.Tests
{
    namespace Connector_Specs
    {
        using System.Threading;
        using System.Threading.Tasks;
        using MassTransit.Pipeline;
        using NUnit.Framework;
        using Pipeline;
        using Policies;
        using RabbitMQ.Client;
        using RabbitMQ.Client.Exceptions;


        class TestConnectionFilter :
            IFilter<ConnectionContext>
        {
            readonly TaskCompletionSource<ConnectionContext> _called;

            public TestConnectionFilter(CancellationToken testCancellationToken)
            {
                _called = new TaskCompletionSource<ConnectionContext>();
                testCancellationToken.Register(() => _called.TrySetCanceled());
            }


            public Task<ConnectionContext> Called
            {
                get { return _called.Task; }
            }

            public async Task Send(ConnectionContext context, IPipe<ConnectionContext> next)
            {
                _called.TrySetResult(context);
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return inspector.Inspect(this);
            }
        }


        [TestFixture]
        public class When_a_server_exists :
            AsyncTestFixture
        {
            [Test]
            public async void Should_connect()
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = "localhost"
                };

                var testConnectionFilter = new TestConnectionFilter(TestCancellationToken);
                IPipe<ConnectionContext> pipe = Pipe.New<ConnectionContext>(x => x.Filter(testConnectionFilter));

                IRabbitMqConnector connector = new RabbitMqConnector(connectionFactory, Retry.None);
                await connector.Connect(pipe, TestCancellationToken);

                await testConnectionFilter.Called;
            }
        }


        [TestFixture]
        public class When_a_server_does_not_exist :
            AsyncTestFixture
        {
            [Test]
            public void Should_throw_connection_exception()
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = "rocalhost"
                };

                var testConnectionFilter = new TestConnectionFilter(TestCancellationToken);
                IPipe<ConnectionContext> pipe = Pipe.New<ConnectionContext>(x => x.Filter(testConnectionFilter));

                IRabbitMqConnector connector = new RabbitMqConnector(connectionFactory, Retry.None);

                var ex = Assert.Throws<RabbitMqConnectionException>(async () => { await connector.Connect(pipe, TestCancellationToken); });

                Assert.IsInstanceOf<BrokerUnreachableException>(ex.InnerException);
            }

            [Test]
            public void Should_connect()
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = "rocalhost"
                };

                var testConnectionFilter = new TestConnectionFilter(TestCancellationToken);
                IPipe<ConnectionContext> pipe = Pipe.New<ConnectionContext>(x => x.Filter(testConnectionFilter));

                IRabbitMqConnector connector = new RabbitMqConnector(connectionFactory, Retry.Intervals(100,100));

                var ex = Assert.Throws<RabbitMqConnectionException>(async () => { await connector.Connect(pipe, TestCancellationToken); });

                Assert.IsInstanceOf<BrokerUnreachableException>(ex.InnerException);
            }
        }

    }
}