namespace MassTransit.AmazonSqsTransport
{
    using System;


    public static class ClientContextBatchSettings
    {
        static ClientContextBatchSettings()
        {
            MessageLimit = 10;
            BatchLimit = 10;
            SizeLimit = 128 * 1024;
            Timeout = TimeSpan.FromMilliseconds(1);
        }

        public static int MessageLimit { get; set; }
        public static int BatchLimit { get; set; }
        public static int SizeLimit { get; set; }
        public static TimeSpan Timeout { get; set; }

        public static BatchSettings GetBatchSettings()
        {
            return new ClientBatchSettings(Math.Min(10, MessageLimit), BatchLimit, SizeLimit, Timeout);
        }


        class ClientBatchSettings :
            BatchSettings
        {
            public ClientBatchSettings(int messageLimit, int batchLimit, int sizeLimit, TimeSpan timeout)
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
