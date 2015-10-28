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
namespace MassTransit.Pipeline.Filters
{
    using System;
    using System.Threading.Tasks;
    using Events;
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
        readonly Func<Task<ISendTransport>> _getDestinationTransport;
        readonly IPublishEndpointProvider _publishEndpoint;

        public MoveExceptionToTransportFilter(IPublishEndpointProvider publishEndpoint, Uri destinationAddress,
            Func<Task<ISendTransport>> getDestinationTransport)
        {
            _getDestinationTransport = getDestinationTransport;
            _publishEndpoint = publishEndpoint;
            _destinationAddress = destinationAddress;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateFilterScope("moveFault");
            scope.Add("destinationAddress", _destinationAddress);
        }

        async Task IFilter<ExceptionReceiveContext>.Send(ExceptionReceiveContext context, IPipe<ExceptionReceiveContext> next)
        {
            ISendTransport transport = await _getDestinationTransport();

            IPipe<SendContext> pipe = Pipe.Execute<SendContext>(sendContext =>
            {
                sendContext.Headers.Set(MessageHeaders.Reason, "fault");

                Exception exception = context.Exception.GetBaseException();

                sendContext.Headers.Set(MessageHeaders.FaultMessage, exception.Message);
                sendContext.Headers.Set(MessageHeaders.FaultStackTrace, ExceptionUtil.GetStackTrace(exception));

                sendContext.SetHostHeaders();
            });

            if (!context.IsFaulted)
                GenerateFault(context);

            await transport.Move(context, pipe).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        void GenerateFault(ExceptionReceiveContext context)
        {
            Guid? faultedMessageId = context.TransportHeaders.Get("MessageId", default(Guid?));

            ReceiveFault fault = new ReceiveFaultEvent(HostMetadataCache.Host, context.Exception, context.ContentType.MediaType, faultedMessageId);

            var publishEndpoint = _publishEndpoint.CreatePublishEndpoint(context.InputAddress);

            var publishTask = publishEndpoint.Publish(fault, context.CancellationToken);

            context.AddPendingTask(publishTask);

            context.AddPendingTask(context.NotifyFaulted(context.Exception));
        }
    }
}