namespace MassTransit.RabbitMqTransport.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Scheduling;
    using Topology;
    using Util;


    public class DelayedExchangeScheduleMessageProvider :
        IScheduleMessageProvider
    {
        readonly IRabbitMqHostTopology _topology;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public DelayedExchangeScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider, IRabbitMqHostTopology topology)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _topology = topology;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, Task<T> message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var scheduleMessagePipe = new RabbitMqScheduleMessagePipe<T>(scheduledTime, pipe);

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

        Task<ISendEndpoint> GetSchedulerEndpoint(Uri address)
        {
            return _sendEndpointProvider.GetSendEndpoint(_topology.GetDelayedExchangeDestinationAddress(address));
        }
    }
}
