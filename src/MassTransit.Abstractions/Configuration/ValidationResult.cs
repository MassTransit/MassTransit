namespace MassTransit
{
    /// <summary>
    /// Reports information about the configuration before configuring
    /// so that corrections can be made without allocating resources, etc.
    /// </summary>
    public interface ValidationResult
    {
        /// <summary>
        /// The disposition of the result, any Failure items will prevent
        /// the configuration from completing.
        /// </summary>
        ValidationResultDisposition Disposition { get; }

        /// <summary>
        /// The message associated with the result
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The key associated with the result (chained if configurators are nested)
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The value associated with the result
        /// </summary>
        string? Value { get; }
    }
}
