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
    using Amazon.SimpleNotificationService.Model;
    using Contexts;
    using GreenPipes;


    public class AmazonSqsMoveTransport
    {
        readonly string _destination;
        readonly IFilter<ModelContext> _topologyFilter;

        protected AmazonSqsMoveTransport(string destination, IFilter<ModelContext> topologyFilter)
        {
            _topologyFilter = topologyFilter;
            _destination = destination;
        }

        protected async Task Move(ReceiveContext context, Action<PublishRequest, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out ModelContext modelContext))
                throw new ArgumentException("The ReceiveContext must contain a BrokeredMessageContext (from Azure Service Bus)", nameof(context));

            await _topologyFilter.Send(modelContext, Pipe.Empty<ModelContext>()).ConfigureAwait(false);

            var topicArn = await modelContext.GetTopic(_destination).ConfigureAwait(false);
            var message = modelContext.CreateTransportMessage(topicArn, context.GetBody());

            if (context.TryGetPayload(out AmazonSqsMessageContext messageContext))
                foreach (string key in messageContext.Attributes.Keys)
                {
                    message.MessageAttributes[key].StringValue = messageContext.Attributes[key].StringValue;
                    message.MessageAttributes[key].BinaryValue = messageContext.Attributes[key].BinaryValue;
                    message.MessageAttributes[key].DataType = messageContext.Attributes[key].DataType;
                }

            SendHeaders headers = new AmazonSnsHeaderAdapter(message.MessageAttributes);

            headers.SetHostHeaders();

            preSend(message, headers);

            var task = Task.Run(() => modelContext.Publish(message));

            context.AddReceiveTask(task);
        }
    }
}
