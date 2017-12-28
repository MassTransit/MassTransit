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
namespace MassTransit.Tests.Topology
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class Creating_a_topology
    {
        [Test]
        public async Task Should_have_a_familiar_syntax()
        {
            MessageCorrelation.UseCorrelationId<LegacyMessage>(x => x.TransactionId);

            var harness = new InMemoryTestHarness();
            harness.OnConfigureInMemoryBus += configurator =>
            {
                configurator.Send<IEvent>(x =>
                {
                    x.UseCorrelationId(p => p.TransactionId);
                });
            };

            Task<ConsumeContext<INewUserEvent>> handled = null;
            Task<ConsumeContext<OtherMessage>> otherHandled = null;
            Task<ConsumeContext<LegacyMessage>> legacyHandled = null;
            harness.OnConfigureInMemoryReceiveEndpoint += configurator =>
            {
                handled = harness.Handled<INewUserEvent>(configurator);
                otherHandled = harness.Handled<OtherMessage>(configurator);
                legacyHandled = harness.Handled<LegacyMessage>(configurator);
            };


            await harness.Start();
            try
            {
                var transactionId = NewId.NextGuid();

                await harness.InputQueueSendEndpoint.Send<INewUserEvent>(new
                {
                    TransactionId = transactionId
                });

                ConsumeContext<INewUserEvent> context = await handled;

                Assert.IsTrue(context.CorrelationId.HasValue);
                Assert.That(context.CorrelationId.Value, Is.EqualTo(transactionId));

                await harness.InputQueueSendEndpoint.Send<OtherMessage>(new
                {
                    CorrelationId = transactionId
                });

                ConsumeContext<OtherMessage> otherContext = await otherHandled;

                Assert.IsTrue(otherContext.CorrelationId.HasValue);
                Assert.That(otherContext.CorrelationId.Value, Is.EqualTo(transactionId));

                await harness.InputQueueSendEndpoint.Send<LegacyMessage>(new
                {
                    TransactionId = transactionId
                });

                ConsumeContext<LegacyMessage> legacyContext = await legacyHandled;

                Assert.IsTrue(legacyContext.CorrelationId.HasValue);
                Assert.That(legacyContext.CorrelationId.Value, Is.EqualTo(transactionId));
            }
            finally
            {
                await harness.Stop();
            }
        }


        public interface IEvent
        {
            Guid TransactionId { get; }
        }


        public interface INewUserEvent :
            IEvent
        {
        }


        public class OtherMessage
        {
            public Guid CorrelationId { get; set; }
        }


        public class LegacyMessage
        {
            public Guid TransactionId { get; set; }
        }
    }
}