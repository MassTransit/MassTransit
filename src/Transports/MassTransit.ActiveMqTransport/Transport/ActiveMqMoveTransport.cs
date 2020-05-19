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
namespace MassTransit.ActiveMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Contexts;
    using GreenPipes;


    public class ActiveMqMoveTransport
    {
        readonly string _destination;
        readonly IFilter<SessionContext> _topologyFilter;

        protected ActiveMqMoveTransport(string destination, IFilter<SessionContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _destination = destination;
        }

        protected async Task Move(ReceiveContext context, Action<IMessage, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out SessionContext sessionContext))
                throw new ArgumentException("The ReceiveContext must contain a SessionContext", nameof(context));

            await _topologyFilter.Send(sessionContext, Pipe.Empty<SessionContext>()).ConfigureAwait(false);

            var queue = await sessionContext.GetQueue(_destination).ConfigureAwait(false);

            var producer = await sessionContext.CreateMessageProducer(queue).ConfigureAwait(false);
            byte[] body = context.GetBody();

            var message = producer.CreateBytesMessage();

            if (context.TryGetPayload(out ActiveMqMessageContext messageContext))
                foreach (string key in messageContext.Properties.Keys)
                    message.Properties[key] = messageContext.Properties[key];

            SendHeaders headers = new ActiveMqHeaderAdapter(message.Properties);

            headers.SetHostHeaders();

            preSend(message, headers);

            message.Content = body;

            var task = Task.Run(() => producer.Send(message));
            context.AddReceiveTask(task);
        }
    }
}
