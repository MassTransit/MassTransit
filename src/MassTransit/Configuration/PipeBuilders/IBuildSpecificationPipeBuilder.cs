namespace MassTransit.PipeBuilders
{
    using GreenPipes;


    public interface IBuildSpecificationPipeBuilder<T> :
        ISpecificationPipeBuilder<T>
        where T : class, PipeContext
    {
        IPipe<T> Build();
        IPipe<T> Build(IPipe<T> lastPipe);
    }
}
