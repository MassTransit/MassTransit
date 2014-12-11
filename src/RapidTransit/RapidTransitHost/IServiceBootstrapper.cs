namespace RapidTransit
{
    using Topshelf;


    public interface IServiceBootstrapper
    {
        string LifetimeScopeTag { get; }
        string ServiceName { get; }
        ServiceControl CreateService();
    }
}