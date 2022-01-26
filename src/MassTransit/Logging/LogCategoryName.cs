#nullable enable
namespace MassTransit.Logging
{
    public static class LogCategoryName
    {
        public const string MassTransit = "MassTransit";


        public static class Transport
        {
            public const string Receive = "MassTransit.ReceiveTransport";
            public const string Send = "MassTransit.SendTransport";
        }
    }
}
