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
namespace MassTransit.AmazonSqsTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Contexts;
    using GreenPipes;


    public class SqsMoveTransport
    {
        readonly string _destination;
        readonly IFilter<ClientContext> _topologyFilter;

        protected SqsMoveTransport(string destination, IFilter<ClientContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _destination = destination;
        }

        protected async Task Move(ReceiveContext context, Action<SendMessageRequest, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out ClientContext clientContext))
                throw new ArgumentException("The ReceiveContext must contain a ClientContext (from Amazon SQS)", nameof(context));

            await _topologyFilter.Send(clientContext, Pipe.Empty<ClientContext>()).ConfigureAwait(false);

            var message = clientContext.CreateSendRequest(_destination, context.GetBody());

            if (context.TryGetPayload(out AmazonSqsMessageContext messageContext))
            {
                foreach (var key in messageContext.Attributes.Keys)
                    message.MessageAttributes[key] = messageContext.Attributes[key];
            }

            SendHeaders headers = new AmazonSqsHeaderAdapter(message.MessageAttributes);

            headers.SetHostHeaders();

            preSend(message, headers);

            var task = Task.Run(() => clientContext.SendMessage(message, context.CancellationToken));

            context.AddReceiveTask(task);
        }
    }
}