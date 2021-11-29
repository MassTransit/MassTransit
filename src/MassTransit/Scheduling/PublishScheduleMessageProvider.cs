namespace MassTransit.Scheduling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class PublishScheduleMessageProvider :
        BaseScheduleMessageProvider
    {
        readonly IPublishEndpoint _publishEndpoint;

        public PublishScheduleMessageProvider(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        protected override Task ScheduleSend(ScheduleMessage message, IPipe<SendContext<ScheduleMessage>> pipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, pipe, cancellationToken);
        }

        protected override Task CancelScheduledSend(Guid tokenId, Uri destinationAddress)
        {
            return _publishEndpoint.Publish<CancelScheduledMessage>(new
            {
                InVar.CorrelationId,
                InVar.Timestamp,
                TokenId = tokenId
            });
        }
    }
}
