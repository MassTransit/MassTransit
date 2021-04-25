namespace MassTransit.Transports
{
    public enum BusState
    {
        Created = 0,
        Started = 1,
        Faulted = 2,
        Stopped = 3,
    }
}
