namespace MassTransit.Initializers.Contexts
{
    using System.Threading;
    using GreenPipes;
    using GreenPipes.Payloads;


    public class BaseInitializeContext :
        BasePipeContext,
        InitializeContext
    {
        public BaseInitializeContext(CancellationToken cancellationToken)
            : base(new PayloadCache(), cancellationToken)
        {
        }

        public BaseInitializeContext(PipeContext context)
            : base(new PayloadCacheScope(context), context.CancellationToken)
        {
        }

        public InitializeContext<T> CreateMessageContext<T>(T message)
            where T : class
        {
            return new DynamicInitializeContext<T>(this, message);
        }
    }
}
