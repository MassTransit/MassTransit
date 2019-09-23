namespace MassTransit.Initializers.Contexts
{
    public class DynamicInitializeContext<TMessage> :
        BaseInitializeContext,
        InitializeContext<TMessage>
        where TMessage : class
    {
        public DynamicInitializeContext(InitializeContext context, TMessage message)
            : base(context)
        {
            Message = message;

            Depth = context.Depth + 1;
            Parent = context;
        }

        public TMessage Message { get; }

        public override int Depth { get; }

        public override InitializeContext Parent { get; }

        public InitializeContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class
        {
            return new DynamicInitializeContext<TMessage, T>(this, Message, input);
        }

        public override bool TryGetParent<T>(out InitializeContext<T> parentContext)
        {
            if (this is InitializeContext<T> parent || (Parent != null && Parent.TryGetParent(out parent)))
            {
                parentContext = parent;
                return true;
            }

            parentContext = default;
            return false;
        }
    }


    public class DynamicInitializeContext<TMessage, TInput> :
        DynamicInitializeContext<TMessage>,
        InitializeContext<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        public DynamicInitializeContext(InitializeContext context, TMessage message, TInput input)
            : base(context, message)
        {
            HasInput = (Input = input) != null;
        }

        public bool HasInput { get; }
        public TInput Input { get; }
    }
}
