// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    [TestFixture]
    public class TopologyRoutingKey_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_handle_base_event_class()
        {
            var transactionId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<IRoutedEvent>(new
            {
                TransactionId = transactionId
            });

            ConsumeContext<IRoutedEvent> context = await _handled;

            Assert.IsTrue(context.CorrelationId.HasValue);
            Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));

            Assert.IsTrue(context.TryGetPayload<RabbitMqBasicConsumeContext>(out var basicConsumeContext));
            Assert.That(basicConsumeContext.RoutingKey, Is.EqualTo(transactionId.ToString()));
        }

        Task<ConsumeContext<IRoutedEvent>> _handled;

        protected override void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            configurator.Send<IRoutedEvent>(x => x.UseRoutingKeyFormatter(context => context.Message.TransactionId.ToString()));

            configurator.Send<IEvent>(x =>
            {
                x.UseCorrelationId(p => p.TransactionId);
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<IRoutedEvent>(configurator);
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface IRoutedEvent :
            IEvent
        {
        }
    }
}