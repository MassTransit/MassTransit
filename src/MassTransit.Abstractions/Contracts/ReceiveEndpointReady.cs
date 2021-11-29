namespace MassTransit
{
    public interface ReceiveEndpointReady :
        ReceiveEndpointEvent
    {
        /// <summary>
        /// If true, the receive endpoint is actually ready, versus "fake-ready" for endpoints which do not auto-start
        /// </summary>
        bool IsStarted { get; }
    }
}
