namespace MassTransit.Initializers
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A message initializer that doesn't use the input
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface IMessageInitializer<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Create a message context, using <paramref name="context" /> as a base for payloads, etc.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        InitializeContext<TMessage> Create(PipeContext context);

        /// <summary>
        /// Create a message context
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        InitializeContext<TMessage> Create(CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message, using the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<InitializeContext<TMessage>> Initialize(object input, CancellationToken cancellationToken);

        /// <summary>
        /// Initialize the message, using the input
        /// </summary>
        /// <param name="context">An existing initialize message context</param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<InitializeContext<TMessage>> Initialize(InitializeContext<TMessage> context, object input);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="context">The base context</param>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, IPipe<SendContext<TMessage>>? pipe = null);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="context">The base context</param>
        /// <param name="input">The input object</param>
        /// <param name="moreInputs">Additional objects used to initialize the message</param>
        /// <param name="pipe"></param>
        /// <returns></returns>
        Task<SendTuple<TMessage>> InitializeMessage(PipeContext context, object input, object[] moreInputs, IPipe<SendContext<TMessage>>? pipe = null);

        /// <summary>
        /// Initialize the message using the input and send it to the endpoint.
        /// </summary>
        /// <param name="input">The input object</param>
        /// <param name="pipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SendTuple<TMessage>> InitializeMessage(object input, IPipe<SendContext<TMessage>> pipe, CancellationToken cancellationToken);
    }
}
