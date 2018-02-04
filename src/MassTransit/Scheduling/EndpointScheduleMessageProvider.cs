namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class EndpointScheduleMessageProvider :
        BaseScheduleMessageProvider
    {
        readonly Func<Task<ISendEndpoint>> _schedulerEndpoint;

        public EndpointScheduleMessageProvider(Func<Task<ISendEndpoint>> schedulerEndpoint)
        {
            _schedulerEndpoint = schedulerEndpoint;
        }

        protected override async Task ScheduleSend<T>(ScheduleMessage<T> message, IPipe<SendContext<ScheduleMessage<T>>> pipe,
            CancellationToken cancellationToken)
        {
            var endpoint = await _schedulerEndpoint().ConfigureAwait(false);

            await endpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task CancelScheduledSend(Guid tokenId, Uri destinationAddress)
        {
            var endpoint = await _schedulerEndpoint().ConfigureAwait(false);

            await endpoint.Send<CancelScheduledMessage>(new
                {
                    InVar.CorrelationId,
                    InVar.Timestamp,
                    TokenId = tokenId,
                })
                .ConfigureAwait(false);
        }
    }
}
