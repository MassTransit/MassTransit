namespace MassTransit
{
    using System.ComponentModel;
    using Configuration;


    /// <summary>
    /// Configures a pipe with specifications
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IPipeConfigurator<TContext>
        where TContext : class, PipeContext
    {
        /// <summary>
        /// Adds a pipe specification to the pipe configurator at the end of the chain
        /// </summary>
        /// <param name="specification">The pipe specification to add</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddPipeSpecification(IPipeSpecification<TContext> specification);
    }
}
