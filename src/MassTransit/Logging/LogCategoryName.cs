namespace MassTransit.Logging
{
    public static class LogCategoryName
    {
        public const string MassTransit = "MassTransit";

        public static class Configuration
        {
            public static class ReceiveEndpoint
            {
                public const string Consumer = "Configuration.ReceiveEndpoint.Consumer";
                public const string Saga = "Configuration.ReceiveEndpoint.Saga";
                public const string ExecuteActivity = "Configuration.ReceiveEndpoint.ExecuteActivity";
                public const string CompensateActivity = "Configuration.ReceiveEndpoint.CompensateActivity";
            }
        }


        public static class Transport
        {
            public const string Receive = "MassTransit.ReceiveTransport";
            public const string Send = "MassTransit.SendTransport";
        }
    }
}
