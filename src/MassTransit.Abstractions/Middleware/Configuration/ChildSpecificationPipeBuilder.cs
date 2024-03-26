namespace MassTransit.Configuration
{
    public partial class PipeConfigurator<TContext>
        where TContext : class, PipeContext
    {
        public class ChildSpecificationPipeBuilder :
            ISpecificationPipeBuilder<TContext>
        {
            readonly ISpecificationPipeBuilder<TContext> _builder;

            public ChildSpecificationPipeBuilder(ISpecificationPipeBuilder<TContext> builder, bool isImplemented, bool isDelegated)
            {
                _builder = builder;

                IsDelegated = isDelegated;
                IsImplemented = isImplemented;
            }

            public void AddFilter(IFilter<TContext> filter)
            {
                _builder.AddFilter(filter);
            }

            public bool IsDelegated { get; }

            public bool IsImplemented { get; }

            public ISpecificationPipeBuilder<TContext> CreateDelegatedBuilder()
            {
                return new ChildSpecificationPipeBuilder(this, IsImplemented, true);
            }

            public ISpecificationPipeBuilder<TContext> CreateImplementedBuilder()
            {
                return new ChildSpecificationPipeBuilder(this, true, IsDelegated);
            }
        }
    }
}
