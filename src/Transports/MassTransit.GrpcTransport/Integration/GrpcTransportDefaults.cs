namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using Grpc.Core;


    public static class GrpcTransportDefaults
    {
        static GrpcTransportDefaults()
        {
            Capacity = 1000;
            MinAge = TimeSpan.FromSeconds(10);
            MaxAge = TimeSpan.FromHours(24);
        }

        public static int Capacity { get; set; }
        public static TimeSpan MinAge { get; set; }
        public static TimeSpan MaxAge { get; set; }
        public static int DefaultPort { get; set; } = ServerPort.PickUnused;
    }
}