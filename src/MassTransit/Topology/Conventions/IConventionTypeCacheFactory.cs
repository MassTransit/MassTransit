namespace MassTransit.Topology.Conventions
{
    public interface IConventionTypeCacheFactory<out TValue>
        where TValue : class
    {
        TValue Create<T>()
            where T : class;
    }
}
