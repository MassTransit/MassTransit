namespace MassTransit.Scheduling
{
    using System;


    public interface IScheduleTokenIdCache<in T>
        where T : class
    {
        /// <summary>
        /// Try to get the tokenId for the scheduler from the message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        bool TryGetTokenId(T message, out Guid tokenId);
    }
}
