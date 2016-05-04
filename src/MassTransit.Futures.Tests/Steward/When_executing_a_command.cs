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
    using NUnit.Framework;


    [TestFixture]
    public class When_executing_a_command :
        InMemoryDispatchTestFixture
    {
        [Test]
        public async void Should_execute()
        {
            Task<ConsumeContext<MagicMade>> received = SubscribeHandler<MagicMade>();

            Uri commandUri = GetCommandContext<MakeMagicHappen>().ExecuteUri;
            var command = new MakeMagicHappenCommand("Hello, World.");

            DispatchMessageHandle<MakeMagicHappenCommand> handle = await DispatchEndpoint.DispatchMessage(command, commandUri);

            ConsumeContext<DispatchAccepted> accepted = await _accepted;
            Assert.AreEqual(handle.DispatchId, accepted.Message.DispatchId);

            await received;
        }

        Task<ConsumeContext<DispatchAccepted>> _accepted;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _accepted = Handled<DispatchAccepted>(configurator);
        }


        public class MakeMagicHappenCommand :
            MakeMagicHappen
        {
            public MakeMagicHappenCommand(string text)
            {
                Text = text;
            }

            public string Text { get; private set; }
        }


        protected override void SetupCommands()
        {
            AddCommandContext<MagicMakingConsumer, MakeMagicHappen>();
        }
    }


    public interface MakeMagicHappen
    {
        string Text { get; }
    }


    class MagicMade
    {
    }


    class MagicMakingConsumer :
        IConsumer<MakeMagicHappen>
    {
        public Task Consume(ConsumeContext<MakeMagicHappen> context)
        {
            return context.Publish(new MagicMade());
        }
    }
}