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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using Transports;
    using Util;


    /// <summary>
    /// In the case of an exception, the message is moved to the destination transport. If the receive had not yet been
    /// faulted, a fault is generated.
    /// </summary>
    public class MoveExceptionToTransportFilter :
        IFilter<ExceptionReceiveContext>
    {
        readonly Uri _destinationAddress;

        public MoveExceptionToTransportFilter(Uri destinationAddress)
        {
            _destinationAddress = destinationAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("moveFault");
            scope.Add("destinationAddress", _destinationAddress);
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            var transport = await context.SendTransportProvider.GetSendTransport(_destinationAddress).ConfigureAwait(false);

            var exception = context.Exception.GetBaseException() ?? context.Exception;

            var exceptionMessage = ExceptionUtil.GetMessage(exception);

            IPipe<SendContext> pipe = Pipe.Execute<SendContext>(sendContext =>
            {
                sendContext.Headers.Set(MessageHeaders.Reason, "fault");

                sendContext.Headers.Set(MessageHeaders.FaultMessage, exceptionMessage);
                sendContext.Headers.Set(MessageHeaders.FaultTimestamp, context.ExceptionTimestamp.ToString("O"));
                sendContext.Headers.Set(MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));

                // avoid faulted TTL messages from disappearing from the error queue
                sendContext.TimeToLive = default(TimeSpan?);

                sendContext.SetHostHeaders();
            });

            if (!context.IsFaulted)
                GenerateFault(context);

            await transport.Move(context, pipe).ConfigureAwait(false);

            context.LogMoved(_destinationAddress, $"Fault: {exceptionMessage}");

            await next.Send(context).ConfigureAwait(false);
        }

        void GenerateFault(ExceptionReceiveContext context)
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