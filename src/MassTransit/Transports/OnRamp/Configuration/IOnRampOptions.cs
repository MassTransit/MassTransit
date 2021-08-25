namespace MassTransit.Transports.Outbox.Configuration
{
    /// <summary>
    /// Settings need for the Send/Publish Transport
    /// </summary>
    public interface IOnRampOptions
    {
        string OnRampName { get; }
    }
}
