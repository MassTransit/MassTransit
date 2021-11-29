namespace MassTransit.Transformation
{
    /// <summary>
    /// A transform property context, which includes the <see cref="TransformContext" />, as well as the current input property value, if present.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface TransformPropertyContext<out TProperty, out TMessage> :
        TransformContext<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// True if the value is present from the source
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// The value
        /// </summary>
        TProperty Value { get; }
    }
}
