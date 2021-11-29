namespace MassTransit.Initializers
{
    using System;


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
        Type MessageType { get; }

        /// <summary>
        /// The message being initialized
        /// </summary>
        TMessage Message { get; }

        InitializeContext<TMessage, T> CreateInputContext<T>(T input)
            where T : class;
    }


    public interface InitializeContext :
        PipeContext
    {
        /// <summary>
        /// how deep this context is within the object graph
        /// </summary>
        int Depth { get; }

        /// <summary>
        /// the parent initialize context, which is valid if the type is being initialized
        /// within another type
        /// </summary>
        InitializeContext Parent { get; }

        /// <summary>
        /// Return the closest parent context for the specified type, if present
        /// </summary>
        /// <param name="parentContext"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool TryGetParent<T>(out InitializeContext<T> parentContext)
            where T : class;

        InitializeContext<T> CreateMessageContext<T>(T message)
            where T : class;
    }
}
