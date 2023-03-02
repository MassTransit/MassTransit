namespace MassTransit.AmazonSqsTransport
{
    using System;


    public static class PublishBatchSettings
    {
        static PublishBatchSettings()
        {
            MessageLimit = 5;
            BatchLimit = 5;
            SizeLimit = 128 * 1024;
            Timeout = TimeSpan.FromMilliseconds(1);
        }

        public static int MessageLimit { get; set; }
        public static int BatchLimit { get; set; }
        public static int SizeLimit { get; set; }
        public static TimeSpan Timeout { get; set; }

        public static BatchSettings GetBatchSettings()
        {
            return new SnsPublishBatchSettings(Math.Min(5, MessageLimit), BatchLimit, Math.Min(256 * 1024, SizeLimit), Timeout);
        }


        class SnsPublishBatchSettings :
            BatchSettings
        {
            public SnsPublishBatchSettings(int messageLimit, int batchLimit, int sizeLimit, TimeSpan timeout)
            {
                Enabled = true;
                MessageLimit = messageLimit;
                BatchLimit = batchLimit;
                SizeLimit = sizeLimit;
                Timeout = timeout;
            }

            public bool Enabled { get; set; }
            public int MessageLimit { get; set; }
            public int BatchLimit { get; set; }
            public int SizeLimit { get; set; }
            public TimeSpan Timeout { get; set; }
        }
    }
}
