namespace MassTransit.Configuration
{
    /// <summary>
    /// Configures a pipe builder (typically by adding filters), but allows late binding to the
    /// pipe builder with pre-validation that the operations will succeed.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IPipeSpecification<TContext> :
        ISpecification
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Apply the specification to the builder
        /// </summary>
        /// <param name="builder">The pipe builder</param>
        void Apply(IPipeBuilder<TContext> builder);
    }
}
