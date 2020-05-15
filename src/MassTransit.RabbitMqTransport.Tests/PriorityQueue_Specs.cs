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
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_priority_queue :
        RabbitMqTestFixture
    {
        Uri _endpointAddress;

        [Test]
        public async Task Should_allow_priority_to_be_specified()
        {
            await Bus.Request<PingMessage, PongMessage>(_endpointAddress, new PingMessage(), TestCancellationToken, TestTimeout, x =>
            {
                x.SetPriority(2);
            });
        }

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            base.ConfigureRabbitMqBusHost(configurator, host);

            configurator.ReceiveEndpoint("priority_input_queue", x =>
            {
                x.EnablePriority(4);

                _endpointAddress = x.InputAddress;

                x.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
            });
        }
    }
}
