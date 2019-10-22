namespace MassTransit.AutofacIntegration
{
    /// <summary>
    /// Used to obtain the lifetime scopeId for the given context
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface ILifetimeScopeIdProvider<TId>
    {
        bool TryGetScopeId(out TId id);
    }
}
