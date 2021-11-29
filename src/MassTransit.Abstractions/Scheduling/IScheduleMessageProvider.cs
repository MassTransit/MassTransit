namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IScheduleMessageProvider
    {
        /// <summary>
        /// Schedule a message to be sent
        /// </summary>
        /// <param name="destinationAddress"></param>
        /// <param name="scheduledTime"></param>
        /// <param name="message"></param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<ScheduledMessage<T>> ScheduleSend<T>(Uri destinationAddress, DateTime scheduledTime, T message, IPipe<SendContext<T>> pipe,
            CancellationToken cancellationToken)
            where T : class;

        /// <summary>
        /// Cancel a scheduled message by TokenId
        /// </summary>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        Task CancelScheduledSend(Guid tokenId);

        /// <summary>
        /// Cancel a scheduled message by TokenId
        /// </summary>
        /// <param name="destinationAddress">The destination address of the scheduled message</param>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        Task CancelScheduledSend(Uri destinationAddress, Guid tokenId);
    }
}
