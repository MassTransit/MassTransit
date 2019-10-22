namespace MassTransit.AutofacIntegration
{
    using Autofac;


    /// <summary>
    /// Delegate to configure a lifetime scope
    /// </summary>
    /// <typeparam name="TId">The container registry identifier type</typeparam>
    /// <param name="scopeId">The scopeId</param>
    /// <param name="containerBuilder">The container builder which configures the lifetime scope</param>
    public delegate void LifetimeScopeConfigurator<in TId>(TId scopeId, ContainerBuilder containerBuilder);
}
