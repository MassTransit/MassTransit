namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    /// <summary>
    /// Converts a <see cref="Task{T}"/> to {T} by awaiting the result and then applying a subsequent conversion
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class AsyncConvertPropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult, Task<TInput>>
    {
        readonly ITypeConverter<TResult, TInput> _converter;

        public AsyncConvertPropertyConverter(ITypeConverter<TResult, TInput> converter)
        {
            _converter = converter;
        }

        Task<TResult> IPropertyConverter<TResult, Task<TInput>>.Convert<TMessage>(InitializeContext<TMessage> context, Task<TInput> input)
        {
            if (input == null)
                return default;

            if (input.IsCompleted)
                return Task.FromResult(_converter.TryConvert(input.Result, out var result) ? result : default);

            return ConvertAsync(input);
        }

        async Task<TResult> ConvertAsync(Task<TInput> input)
        {
            var inputValue = await input.ConfigureAwait(false);

            return _converter.TryConvert(inputValue, out var result) ? result : default;
        }
    }
}
