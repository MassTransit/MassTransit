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
namespace MassTransit.Tests.Steward
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Steward;
    using MassTransit.Steward.Contracts;
    using MassTransit.Steward.Contracts.Events;
    using NUnit.Framework;


    [TestFixture]
    public class When_contacting_an_http_url :
        InMemoryDispatchTestFixture
    {
        [Test]
        public async Task Should_succeed_nicely()
        {
            Task<ConsumeContext<HttpRequestSucceeded>> succeeded = SubscribeHandler<HttpRequestSucceeded>();
            Task<ConsumeContext<HttpRequestFaulted>> faulted = SubscribeHandler<HttpRequestFaulted>();
            Task<ConsumeContext<ResourceUsageCompleted>> completed = SubscribeHandler<ResourceUsageCompleted>();


            Uri commandUri = GetCommandContext<ExecuteHttpRequest>().ExecuteUri;

            var webUrl = new Uri("https://help.github.com/");

            var command = new ExecuteHttpRequestCommand(webUrl);


            DispatchMessageHandle<ExecuteHttpRequest> handle = await DispatchEndpoint.DispatchMessage(command, commandUri, webUrl);

            ConsumeContext<DispatchAccepted> accepted = await _accepted;

            Assert.AreEqual(handle.DispatchId, accepted.Message.DispatchId);

            Task waitAny = await Task.WhenAny(succeeded, faulted);

            Console.WriteLine("Request Duration: {0}", (await completed).Message.Duration);
        }

        Task<ConsumeContext<DispatchAccepted>> _accepted;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _accepted = Handled<DispatchAccepted>(configurator);

            base.ConfigureInputQueueEndpoint(configurator);
        }

        protected override void SetupCommands()
        {
            AddCommandContext<HttpProxyConsumer, ExecuteHttpRequest>();
        }


        class ExecuteHttpRequestCommand :
            ExecuteHttpRequest
        {
            public ExecuteHttpRequestCommand(Uri url)
            {
                Url = url;
            }

            public Uri Url { get; private set; }
        }
    }
}