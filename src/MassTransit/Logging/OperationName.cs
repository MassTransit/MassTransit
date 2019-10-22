namespace MassTransit.Logging
{
    public static class OperationName
    {
        public static class Transport
        {
            public const string Send = "Transport.Send";
            public const string Receive = "Transport.Receive";
        }


        public static class Consumer
        {
            public const string Consume = "Consumer.Consume";
            public const string Handle = "Consumer.Handle";
        }


        public static class Saga
        {
            public const string Send = "Saga.Send";
            public const string SendQuery = "Saga.SendQuery";
            public const string Add = "Saga.Add";
            public const string RaiseEvent = "Saga.RaiseEvent";
        }

        public static class Courier
        {
            public const string Execute = "Activity.Execute";
            public const string Compensate = "Activity.Compensate";
        }
    }
}
