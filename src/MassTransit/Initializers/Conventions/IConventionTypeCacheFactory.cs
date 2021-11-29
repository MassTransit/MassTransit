namespace MassTransit.Initializers.Conventions
{
    public interface IConventionTypeCacheFactory<out TValue>
        where TValue : class
    {
        TValue Create<T>(IInitializerConvention convention)
            where T : class;
    }
}
