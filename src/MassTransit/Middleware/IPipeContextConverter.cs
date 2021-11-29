namespace MassTransit.Middleware
{
    /// <summary>
    /// Converts the input context to the output context
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IPipeContextConverter<in TInput, TOutput>
        where TInput : class, PipeContext
        where TOutput : class, PipeContext
    {
        bool TryConvert(TInput input, out TOutput output);
    }
}
