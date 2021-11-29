namespace MassTransit.Configuration
{
    public class ChildSpecificationPipeBuilder<T> :
        ISpecificationPipeBuilder<T>
        where T : class, PipeContext
    {
        readonly ISpecificationPipeBuilder<T> _builder;

        public ChildSpecificationPipeBuilder(ISpecificationPipeBuilder<T> builder, bool isImplemented, bool isDelegated)
        {
            _builder = builder;

            IsDelegated = isDelegated;
            IsImplemented = isImplemented;
        }

        public void AddFilter(IFilter<T> filter)
        {
            _builder.AddFilter(filter);
        }

        public bool IsDelegated { get; }

        public bool IsImplemented { get; }

        public ISpecificationPipeBuilder<T> CreateDelegatedBuilder()
        {
            return new ChildSpecificationPipeBuilder<T>(this, IsImplemented, true);
        }

        public ISpecificationPipeBuilder<T> CreateImplementedBuilder()
        {
            return new ChildSpecificationPipeBuilder<T>(this, true, IsDelegated);
        }
    }
}
