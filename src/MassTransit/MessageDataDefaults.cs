namespace MassTransit
{
    using System;


    public static class MessageDataDefaults
    {
        static MessageDataDefaults()
        {
            AlwaysWriteToRepository = true;

            Threshold = 4096;
        }

        /// <summary>
        /// Transitional, will always write to the repository but will include inline to avoid reading on
        /// current framework clients. If all services are upgraded, set to false so that data sizes below
        /// the threshold are not written to the repository.
        /// </summary>
        public static bool AlwaysWriteToRepository { get; set; }

        /// <summary>
        /// Set the threshold for automatic message data to be written to the repository, vs stored inline.
        /// </summary>
        public static int Threshold { get; set; }

        /// <summary>
        /// Set the default time to live for message data when no expiration is specified
        /// </summary>
        public static TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// Set an extra time to live for message data, which is added to inferred expiration based upon
        /// SendContext TimeToLive.
        /// </summary>
        public static TimeSpan? ExtraTimeToLive { get; set; }
    }
}
