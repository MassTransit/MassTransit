namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;

    public interface ConnectionMessageData<THub> where THub : Hub
    {
        string ConnectionId { get; set; }
        MessageData<byte[]> Messages { get; set; }
    }
}
