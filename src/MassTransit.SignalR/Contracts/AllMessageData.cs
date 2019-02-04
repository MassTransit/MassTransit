namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;

    public interface AllMessageData<THub> where THub : Hub
    {
        string[] ExcludedConnectionIds { get; set; }
        MessageData<byte[]> Messages { get; set; }
    }
}
