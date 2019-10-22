namespace MassTransit.Initializers.Contexts
{
    using GreenPipes;


    public class ProxyInitializeContext :
        ProxyPipeContext,
        InitializeContext
    {
        public ProxyInitializeContext(PipeContext context)
            : base(context)
        {
        }

        public virtual int Depth => 0;

        public virtual InitializeContext Parent => null;

        public virtual bool TryGetParent<T>(out InitializeContext<T> parentContext)
            where T : class
        {
            parentContext = default;
            return false;
        }

        public InitializeContext<T> CreateMessageContext<T>(T message)
            where T : class
        {
            return new DynamicInitializeContext<T>(this, message);
        }
    }
}
