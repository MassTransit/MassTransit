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
namespace MassTransit.ActiveMqTransport.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Topology;
    using Topology.Settings;
    using Util;

    public class DelayedExchangeScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly IActiveMqHostTopology _topology;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly Uri _hostAddress;

        public DelayedExchangeScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider, IActiveMqHostTopology topology, Uri hostAddress)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _topology = topology;
            _hostAddress = hostAddress;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, Task<T> message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var scheduleMessagePipe = new ActiveMqScheduleMessagePipe<T>(scheduledTime, pipe);

            var payload = await message.ConfigureAwait(false);

            var schedulerEndpoint = await GetSchedulerEndpoint(destinationAddress).ConfigureAwait(false);

            await schedulerEndpoint.Send(payload, scheduleMessagePipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(scheduleMessagePipe.ScheduledMessageId ?? NewId.NextGuid(), scheduledTime, destinationAddress, payload);
        }

        public Task CancelScheduledSend(Guid tokenId)
        {
            return TaskUtil.Completed;
        }

        public Task CancelScheduledSend(Uri destinationAddress, Guid tokenId)
        {
            return TaskUtil.Completed;
        }

        Task<ISendEndpoint> GetSchedulerEndpoint(Uri destinationAddress)
        {
            var destinationSettings = _topology.GetSendSettings(destinationAddress);

            var topicName = destinationSettings.EntityName;
            var sendSettings = new QueueSendSettings(topicName, destinationSettings.Durable, destinationSettings.AutoDelete);

            var delayExchangeAddress = sendSettings.GetSendAddress(_hostAddress);

            return _sendEndpointProvider.GetSendEndpoint(delayExchangeAddress);
        }
    }
}
