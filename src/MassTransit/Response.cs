namespace MassTransit
{
    /// <summary>
    /// A request's response, which is of the specified type
    /// </summary>
    /// <typeparam name="TResponse">The response type</typeparam>
    public interface Response<out TResponse> :
        MessageContext
        where TResponse : class
    {
        /// <summary>
        /// The response message that was received
        /// </summary>
        TResponse Message { get; }
    }
}
