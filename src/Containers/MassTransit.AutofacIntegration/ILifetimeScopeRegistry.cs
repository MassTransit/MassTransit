namespace MassTransit.AutofacIntegration
{
    using Autofac;


    /// <summary>
    /// A lifetime scope registry contains an indexed set of lifetime scopes that can be used on 
    /// a per-index basis as the root for additional lifetime scopes (per request, etc.)
    /// </summary>
    public interface ILifetimeScopeRegistry<TId> :
        ILifetimeScope
    {
        /// <summary>
        /// Returns the lifetime scope for the specified scopeId
        /// </summary>
        /// <param name="scopeId">The scope identifier</param>
        /// <returns>The lifetime scope</returns>
        ILifetimeScope GetLifetimeScope(TId scopeId);

        /// <summary>
        /// Specify the configuration method for a lifetime scope
        /// </summary>
        /// <param name="scopeId">The switch identifier</param>
        /// <param name="configureCallback">The configuration action for the switch</param>
        void ConfigureLifetimeScope(TId scopeId, LifetimeScopeConfigurator<TId> configureCallback);
    }
}
