// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.QuartzIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Quartz;
    using Serialization;


    public class ScheduledMessageJob :
        IJob,
        SerializedMessage
    {
        readonly IBus _bus;

        public ScheduledMessageJob(IBus bus)
        {
            _bus = bus;
        }

        public string Destination { get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var destinationAddress = new Uri(Destination);
                var sourceAddress = _bus.Address;

                IPipe<SendContext> sendPipe = CreateMessageContext(sourceAddress, context.Trigger.Key.Name);

                var endpoint = await _bus.GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                var scheduled = new Scheduled();

                await endpoint.Send(scheduled, sendPipe, context.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogContext.Error?.Log(ex, "Failed to send scheduled message, type: {MessageType}, destination: {DestinationAddress}", MessageType, Destination);

                throw new JobExecutionException(ex, context.RefireCount < 5);
            }
        }

        public string ExpirationTime { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string Body { get; set; }
        public string MessageId { get; set; }
        public string MessageType { get; set; }
        public string ContentType { get; set; }
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string ConversationId { get; set; }
        public string InitiatorId { get; set; }
        public string TokenId { get; set; }
        public string HeadersAsJson { get; set; }
        public string PayloadMessageHeadersAsJson { get; set; }

        Uri SerializedMessage.Destination => new Uri(Destination);

        IPipe<SendContext> CreateMessageContext(Uri sourceAddress, string triggerKey)
        {
            IPipe<SendContext> sendPipe = Pipe.New<SendContext>(x =>
            {
                x.UseExecute(context =>
                {
                    Guid? tokenId = ConvertIdToGuid(TokenId);
                    if (tokenId.HasValue)
                    {
                        context.Headers.Set(MessageHeaders.SchedulingTokenId, tokenId.Value.ToString("N"));
                    }

                    context.Headers.Set(MessageHeaders.QuartzTriggerKey, triggerKey);
                });
            });

            return new SerializedMessageContextAdapter(sendPipe, this, sourceAddress);
        }

        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }


        class Scheduled
        {
        }
    }
}
