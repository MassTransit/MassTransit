namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;

    public interface UserMessageData<THub> where THub : Hub
    {
        string UserId { get; set; }
        MessageData<byte[]> Messages { get; set; }
    }
}
