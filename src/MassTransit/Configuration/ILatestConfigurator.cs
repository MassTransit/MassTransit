namespace MassTransit
{
    public interface ILatestConfigurator<T>
        where T : class, PipeContext
    {
        LatestFilterCreated<T> Created { set; }
    }
}
