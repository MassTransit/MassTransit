namespace MassTransit.Monitoring.Performance
{
    using System;


    public interface IMessagePerformanceCounter
    {
        /// <summary>
        /// A message was consumed, including the consume duration
        /// </summary>
        /// <param name="duration"></param>
        void Consumed(TimeSpan duration);

        /// <summary>
        /// A message faulted while being consumed
        /// </summary>
        /// <param name="duration"></param>
        void ConsumeFaulted(TimeSpan duration);

        /// <summary>
        /// A message was sent
        /// </summary>
        void Sent();

        /// <summary>
        /// A message was published
        /// </summary>
        void Published();

        /// <summary>
        /// A publish faulted
        /// </summary>
        void PublishFaulted();

        /// <summary>
        /// A send faulted
        /// </summary>
        void SendFaulted();
    }
}
