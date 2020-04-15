namespace MassTransit.Scoping.PublishContexts
{
    public class ExistingPublishScopeContext<T> :
        IPublishScopeContext<T>
        where T : class
    {
        public ExistingPublishScopeContext(PublishContext<T> context)
        {
            Context = context;
        }

        public void Dispose()
        {
        }

        public PublishContext<T> Context { get; }
    }
}
