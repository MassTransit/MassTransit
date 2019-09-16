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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using Metadata;
    using Serialization;
    using Util;


    /// <summary>
    /// Generates and publishes a <see cref="Fault"/> event for the exception
    /// </summary>
    public class GenerateFaultFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("generateFault");
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            if (!context.IsFaulted)
            {
                GenerateFault(context);

                await context.NotifyFaulted(context.Exception).ConfigureAwait(false);
            }

            await next.Send(context).ConfigureAwait(false);
        }

        static void GenerateFault(ExceptionReceiveContext context)
        {
            IPublishEndpoint publishEndpoint;
            Guid? faultedMessageId;

            if (context.TryGetPayload(out ConsumeContext consumeContext))
            {
                publishEndpoint = consumeContext;
                faultedMessageId = consumeContext.MessageId;
            }
            else
            {
                faultedMessageId = context.TransportHeaders.Get("MessageId", default(Guid?));

                publishEndpoint = context.PublishEndpointProvider.CreatePublishEndpoint(context.InputAddress);
            }

            ReceiveFault fault = new ReceiveFaultEvent(HostMetadataCache.Host, context.Exception, context.ContentType.MediaType, faultedMessageId);

            var contextPipe = new ConsumeSendContextPipe<ReceiveFault>(consumeContext);

            var publishTask = publishEndpoint.Publish(fault, contextPipe, context.CancellationToken);

            context.AddReceiveTask(publishTask);
        }
    }
}