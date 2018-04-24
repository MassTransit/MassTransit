// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using RabbitMQ.Client;


    public class RabbitMqMoveTransport
    {
        readonly string _exchange;
        readonly IFilter<ModelContext> _topologyFilter;

        protected RabbitMqMoveTransport(string exchange, IFilter<ModelContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<IBasicProperties, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out ModelContext modelContext))
                throw new ArgumentException("The ReceiveContext must contain a BrokeredMessageContext (from Azure Service Bus)", nameof(context));

            await _topologyFilter.Send(modelContext, Pipe.Empty<ModelContext>()).ConfigureAwait(false);

            IBasicProperties properties;
            string routingKey = "";
            byte[] body;

            if (context.TryGetPayload(out RabbitMqBasicConsumeContext basicConsumeContext))
            {
                properties = basicConsumeContext.Properties;
                routingKey = basicConsumeContext.RoutingKey;
                body = basicConsumeContext.Body;
            }
            else
            {
                properties = modelContext.Model.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object>();

                body = context.GetBody();
            }

            SendHeaders headers = new RabbitMqSendHeaders(properties);

            headers.SetHostHeaders();

            preSend(properties, headers);

            var task = modelContext.BasicPublishAsync(_exchange, routingKey, true, properties, body, true);
            context.AddPendingTask(task);
        }
    }
}