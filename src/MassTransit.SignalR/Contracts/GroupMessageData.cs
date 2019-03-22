namespace MassTransit.SignalR.Contracts
{
    using Microsoft.AspNetCore.SignalR;

    public interface GroupMessageData<THub> where THub : Hub
    {
        string GroupName { get; set; }
        string[] ExcludedConnectionIds { get; set; }
        MessageData<byte[]> Messages { get; set; }
    }
}
