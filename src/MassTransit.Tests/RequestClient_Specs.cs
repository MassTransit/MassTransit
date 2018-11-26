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
namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_using_the_request_client :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            PongMessage message = await _response;

            message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service :
        InMemoryTestFixture
    {
        [Test]
        public void Should_timeout()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestTimeoutException>());
        }

        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(1));

            _response = _requestClient.Request(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_missing_service_that_times_out :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_timeout()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("UnobservedTaskException: ");
                    eventArgs.SetObserved();
                }
                finally
                {
                    Console.ResetColor();
                }

                Console.WriteLine(eventArgs.Exception);
            };

            Assert.That(async () => await _response, Throws.TypeOf<RequestTimeoutException>());

            GC.Collect();
            await Task.Delay(1000);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = new MessageRequestClient<PingMessage, PongMessage>(Bus, InputQueueAddress, TimeSpan.FromSeconds(1));

            _response = _requestClient.Request(new PingMessage());
        }
    }


    [TestFixture]
    public class Sending_a_request_to_a_faulty_service :
        InMemoryTestFixture
    {
        [Test]
        public void Should_receive_the_exception()
        {
            Assert.That(async () => await _response, Throws.TypeOf<RequestFaultException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();

            _response = _requestClient.Request(new PingMessage());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                throw new InvalidOperationException("This is an expected test failure");
            });
        }
    }


    [TestFixture]
    public class Cancelling_a_request_mid_stream :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_throw_a_cancelled_exception()
        {
            Assert.That(async () => await _response, Throws.TypeOf<TaskCanceledException>());
        }

        Task<ConsumeContext<PingMessage>> _ping;
        Task<PongMessage> _response;
        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();

            var cts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                await Task.Delay(500);
                cts.Cancel();
            });

            _response = _requestClient.Request(new PingMessage(), cts.Token);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x =>
            {
                await Task.Delay(2000);
                await x.RespondAsync(new PongMessage(x.Message.CorrelationId));
            });
        }
    }
}