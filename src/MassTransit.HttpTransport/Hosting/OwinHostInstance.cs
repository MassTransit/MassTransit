namespace MassTransit.HttpTransport.Hosting
{
    using System;
    using MassTransit.Pipeline;


    public interface OwinHostInstance : IDisposable
    {
        string Host { get; }
        int Port { get; set; }
        EventHandler<ShutdownEventArgs> HostShutdown { get; set; }
        void Start(IPipe<ReceiveContext> receivePipe);
    }
}