namespace MassTransit.PipeBuilders
{
    using Pipeline;


    public interface IPipeBuilder<T>
        where T : class, PipeContext
    {
        void AddFilter(IFilter<T> filter);
    }
}