namespace MassTransit.Configuration
{
    /// <summary>
    /// A pipe builder constructs a pipe by adding filter to the end of the chain, after
    /// while the builder completes the pipe/filter combination.
    /// </summary>
    /// <typeparam name="TContext">The pipe context type</typeparam>
    public interface IPipeBuilder<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Add a filter to the pipe after any existing filters
        /// </summary>
        /// <param name="filter">The filter to add</param>
        void AddFilter(IFilter<TContext> filter);
    }
}
