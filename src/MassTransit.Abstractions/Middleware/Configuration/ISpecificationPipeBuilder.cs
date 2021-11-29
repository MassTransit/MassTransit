namespace MassTransit.Configuration
{
    public interface ISpecificationPipeBuilder<T> :
        IPipeBuilder<T>
        where T : class, PipeContext
    {
        /// <summary>
        /// If true, this is a delegated builder, and implemented message types
        /// and/or topology items should not be applied
        /// </summary>
        bool IsDelegated { get; }

        /// <summary>
        /// If true, this is a builder for implemented types, so don't go down
        /// the rabbit hole twice.
        /// </summary>
        bool IsImplemented { get; }

        ISpecificationPipeBuilder<T> CreateDelegatedBuilder();

        ISpecificationPipeBuilder<T> CreateImplementedBuilder();
    }
}
