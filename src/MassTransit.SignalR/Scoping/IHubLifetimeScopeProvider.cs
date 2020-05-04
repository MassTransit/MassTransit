namespace MassTransit.SignalR.Scoping
{
    using Microsoft.AspNetCore.SignalR;


    public interface IHubLifetimeScopeProvider
    {
        IHubLifetimeScope<THub> CreateScope<THub>()
            where THub : Hub;
    }
}
