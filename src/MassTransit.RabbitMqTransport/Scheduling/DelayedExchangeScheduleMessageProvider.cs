namespace MassTransit.RabbitMqTransport.Scheduling
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
        readonly IRabbitMqHostTopology _topology;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly Uri _hostAddress;

        public DelayedExchangeScheduleMessageProvider(ISendEndpointProvider sendEndpointProvider, IRabbitMqHostTopology topology, Uri hostAddress)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _topology = topology;
            _hostAddress = hostAddress;
        }

        public async Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, Task<T> message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class
        {
            var messageId = NewId.NextGuid();

            IPipe<SendContext<T>> delayPipe = Pipe.ExecuteAsync<SendContext<T>>(async context =>
            {
                context.MessageId = messageId;
                var rabbitSendContext = context.GetPayload<RabbitMqSendContext>();

                var delay = Math.Max(0, (scheduledTime.Kind == DateTimeKind.Local
                    ? scheduledTime - DateTime.Now
                    : scheduledTime - DateTime.UtcNow).TotalMilliseconds);

                if (delay > 0)
                    rabbitSendContext.SetTransportHeader("x-delay", (long)delay);

                await pipe.Send(context).ConfigureAwait(false);
            });

            var payload = await message.ConfigureAwait(false);

            var schedulerEndpoint = await GetSchedulerEndpoint(destinationAddress).ConfigureAwait(false);

            await schedulerEndpoint.Send(payload, delayPipe, cancellationToken).ConfigureAwait(false);

            return new ScheduledMessageHandle<T>(messageId, scheduledTime, destinationAddress, payload);
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

            var sendSettings = new RabbitMqSendSettings(destinationSettings.ExchangeName + "_delay", "x-delayed-message", destinationSettings.Durable,
                destinationSettings.AutoDelete);

            sendSettings.SetExchangeArgument("x-delayed-type", destinationSettings.ExchangeType);

            sendSettings.BindToExchange(destinationSettings.ExchangeName);

            var delayExchangeAddress = sendSettings.GetSendAddress(_hostAddress);

            return _sendEndpointProvider.GetSendEndpoint(delayExchangeAddress);
        }
    }
}