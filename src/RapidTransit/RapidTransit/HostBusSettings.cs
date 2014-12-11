namespace RapidTransit
{
    using Configuration;


    public interface HostBusSettings :
        ISettings
    {
        string QueueName { get; }
    }
}