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
namespace MassTransit.Steward.Core.Agents
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using GreenPipes;
    using Serialization;


    public class MessageDispatchAgent :
        DispatchAgent
    {
        async Task DispatchAgent.Execute(DispatchContext context)
        {
            try
            {
                ISendEndpoint endpoint = await context.GetSendEndpoint(context.Destination);

                IPipe<SendContext> messageContext = CreateMessageContext(context);

                var dispatched = new Dispatched();
                await endpoint.Send(dispatched, messageContext);

                PublishCommandForwardedEvent(context);
            }
            catch (Exception ex)
            {
                string message = string.Format(CultureInfo.InvariantCulture,
                    "An exception occurred sending message {0} to {1}", string.Join(",", context.DispatchTypes),
                    context.Destination);

                LogContext.Error?.Log(ex, message);

                throw new DispatchException(message, ex);
            }
        }

        void PublishCommandForwardedEvent(DispatchContext context)
        {
            var @event = new DispatchAcceptedEvent(context);

            context.Publish<DispatchAccepted>(@event);
        }

        IPipe<SendContext> CreateMessageContext(DispatchContext dispatchContext)
        {
            IPipe<SendContext> pipe = dispatchContext.CreateCopyContextPipe((consumeContext, context) =>
            {
                context.SourceAddress = dispatchContext.ReceiveContext.InputAddress;

                context.Serializer = new BodySerializer(context.ContentType, Encoding.UTF8.GetBytes(dispatchContext.Body));
            });

            return pipe;
        }


        class DispatchAcceptedEvent :
            DispatchAccepted
        {
            readonly DispatchContext _context;

            public DispatchAcceptedEvent(DispatchContext context)
            {
                EventId = NewId.NextGuid();
                Timestamp = DateTime.UtcNow;

                _context = context;
            }

            public Guid DispatchId
            {
                get { return _context.DispatchId; }
            }

            public DateTime CreateTime
            {
                get { return _context.CreateTime; }
            }

            public Uri[] Resources
            {
                get { return _context.Resources; }
            }

            public string[] DispatchTypes
            {
                get { return _context.DispatchTypes; }
            }

            public Uri Destination
            {
                get { return _context.Destination; }
            }

            public Guid EventId { get; private set; }
            public DateTime Timestamp { get; private set; }
        }


        class Dispatched
        {
        }
    }
}
