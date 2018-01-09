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
    using Events;
    using GreenPipes;
    using Util;


    /// <summary>
    /// Generates and publishes a <see cref="Fault"/> event for the exception
    /// </summary>
    public class GenerateFaultFilter :
        IFilter<ExceptionReceiveContext>
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("moveFault");
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            if (!context.IsFaulted)
                GenerateFault(context);

            await next.Send(context).ConfigureAwait(false);
        }

        static void GenerateFault(ExceptionReceiveContext context)
        {
            Guid? faultedMessageId = context.TransportHeaders.Get("MessageId", default(Guid?));

            ReceiveFault fault = new ReceiveFaultEvent(HostMetadataCache.Host, context.Exception, context.ContentType.MediaType, faultedMessageId);

            var publishEndpoint = context.PublishEndpointProvider.CreatePublishEndpoint(context.InputAddress);

            var publishTask = publishEndpoint.Publish(fault, context.CancellationToken);

            context.AddPendingTask(publishTask);

            context.AddPendingTask(context.NotifyFaulted(context.Exception));
        }
    }
}