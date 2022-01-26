#nullable enable
namespace MassTransit.Logging
{
    public static class OperationName
    {
        public static class Consumer
        {
            public const string Consume = "MassTransit.Consumer.Consume";
            public const string Handle = "MassTransit.Consumer.Handle";
        }


        public static class Saga
        {
            public const string Send = "MassTransit.Saga.Send";
            public const string SendQuery = "MassTransit.Saga.SendQuery";
            public const string Initiate = "MassTransit.Saga.Initiate";
            public const string Orchestrate = "MassTransit.Saga.Orchestrate";
            public const string InitiateOrOrchestrate = "MassTransit.Saga.InitiateOrOrchestrate";
            public const string Observe = "MassTransit.Saga.Observe";
            public const string RaiseEvent = "MassTransit.Saga.RaiseEvent";
        }


        public static class Courier
        {
            public const string Execute = "MassTransit.Activity.Execute";
            public const string Compensate = "MassTransit.Activity.Compensate";
        }
    }
}
