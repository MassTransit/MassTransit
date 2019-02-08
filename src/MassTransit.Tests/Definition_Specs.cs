// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    namespace PingDefinitions
    {
        using System;
        using System.Threading.Tasks;
        using Definition;
        using GreenPipes;
        using TestFramework.Messages;


        public interface SubmitOrder
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface OrderAccepted
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface OrderRejected
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface OrderReceived
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public interface ProcessOrder
        {
            Guid OrderId { get; }

            string CustomerId { get; }
        }


        public class PingConsumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
            }
        }


        public class SubmitOrderConsumer :
            IConsumer<SubmitOrder>
        {
            public async Task Consume(ConsumeContext<SubmitOrder> context)
            {
            }
        }


        public class SubmitOrderConsumerDefinition :
            ConsumerDefinition<SubmitOrderConsumer>
        {
            public SubmitOrderConsumerDefinition()
            {
                Request<SubmitOrder>(x =>
                {
                    x.PartitionBy(m => m.CustomerId);

                    x.Facet(m => m.CustomerId);

                    x.Responds<OrderAccepted>();
                    x.Responds<OrderRejected>();
                    x.Publishes<OrderReceived>();
                    x.Sends<ProcessOrder>();
                });
            }
        }


    }
}
