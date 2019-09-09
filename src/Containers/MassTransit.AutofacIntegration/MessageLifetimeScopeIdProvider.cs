namespace MassTransit.AutofacIntegration
{
    /// <summary>
    /// Gets the LifetimeScope Id using the message
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class MessageLifetimeScopeIdProvider<TMessage, TId> :
        ILifetimeScopeIdProvider<TId>
        where TMessage : class
    {
        readonly ILifetimeScopeIdAccessor<TMessage, TId> _accessor;
        readonly ConsumeContext<TMessage> _consumeContext;

        public MessageLifetimeScopeIdProvider(ConsumeContext<TMessage> consumeContext, ILifetimeScopeIdAccessor<TMessage, TId> accessor)
        {
            _consumeContext = consumeContext;
            _accessor = accessor;
        }

        public bool TryGetScopeId(out TId id)
        {
            return _accessor.TryGetScopeId(_consumeContext.Message, out id);
        }
    }
}
