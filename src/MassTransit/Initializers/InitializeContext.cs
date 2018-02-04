namespace MassTransit.Initializers
{
    using GreenPipes;


    /// <summary>
    /// Message initialization context, which includes the message being initialized and the input
    /// being used to initialize the message properties.
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    /// <typeparam name="TInput">The input type</typeparam>
    public interface InitializeContext<out TMessage, out TInput> :
        InitializeContext<TMessage>
        where TMessage : class
        where TInput : class
    {
        /// <summary>
        /// If true, the input is present, otherwise it equals <i>default</i>.
        /// </summary>
        bool HasInput { get; }

        TInput Input { get; }
    }


    /// <summary>
    /// The context of the message being initialized
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface InitializeContext<out TMessage> :
        InitializeContext
        where TMessage : class
    {
        InitializeContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class;

        /// <summary>
        /// The message being initialized
        /// </summary>
        TMessage Message { get; }
    }


    public interface InitializeContext :
        PipeContext
    {
        InitializeContext<T> CreateMessageContext<T>(T message)
            where T : class;
    }
}
