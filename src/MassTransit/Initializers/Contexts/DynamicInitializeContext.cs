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
        }

        public TMessage Message { get; }

        public InitializeContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class
        {
            return new DynamicInitializeContext<TMessage, T>(this, Message, input);
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
