namespace MassTransit.Initializers
{
    /// <summary>
    /// A synchronous property type conversion, which may or may not succeed.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public interface ITypeConverter<TResult, in TInput>
    {
        /// <summary>
        /// Convert the input to the result type
        /// </summary>
        /// <param name="input">The input value</param>
        /// <param name="result">The result value</param>
        /// <returns>True if the value was converted, otherwise false</returns>
        bool TryConvert(TInput input, out TResult result);
    }
}
