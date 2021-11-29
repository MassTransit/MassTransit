namespace MassTransit
{
    /// <summary>
    /// Context used by a message transform
    /// </summary>
    public interface TransformContext :
        PipeContext,
        MessageContext
    {
    }


    /// <summary>
    /// A message transform for a single message type
    /// </summary>
    /// <typeparam name="TMessage">The message type</typeparam>
    public interface TransformContext<out TMessage> :
        TransformContext
        where TMessage : class
    {
        /// <summary>
        /// If true, the input is present, otherwise it equals <i>default</i>.
        /// </summary>
        bool HasInput { get; }

        TMessage Input { get; }
    }
}
