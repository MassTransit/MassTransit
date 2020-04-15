namespace MassTransit.Scoping.PublishContexts
{
    using System;


    public class CreatedPublishScopeContext<TScope, T> :
        IPublishScopeContext<T>
        where TScope : IDisposable
        where T : class
    {
        readonly TScope _scope;

        public CreatedPublishScopeContext(TScope scope, PublishContext<T> context)
        {
            _scope = scope;
            Context = context;
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }

        public PublishContext<T> Context { get; }
    }
}
