namespace MassTransit.AutofacIntegration
{
    /// <summary>
    /// Returns the scopeId from an input type
    /// </summary>
    /// <typeparam name="TInput">The input type</typeparam>
    /// <typeparam name="TId">The scopeId type</typeparam>
    public interface ILifetimeScopeIdAccessor<in TInput, TId>
    {
        /// <summary>
        /// Try to retrieve the scopeId from the input
        /// </summary>
        /// <param name="input">The input value</param>
        /// <param name="id">The scopeId</param>
        /// <returns></returns>
        bool TryGetScopeId(TInput input, out TId id);
    }
}
