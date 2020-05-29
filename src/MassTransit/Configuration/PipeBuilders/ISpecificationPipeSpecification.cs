namespace MassTransit.PipeBuilders
{
    using GreenPipes;


    public interface ISpecificationPipeSpecification<T> :
        ISpecification
        where T : class, PipeContext
    {
        void Apply(ISpecificationPipeBuilder<T> builder);
    }
}
