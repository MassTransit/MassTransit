namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    /// <summary>
    /// Converts a <see cref="Task{T}"/> to {T} by awaiting the result and then applying a subsequent conversion
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class AsyncPropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult, Task<TInput>>
    {
        readonly IPropertyConverter<TResult, TInput> _converter;

        public AsyncPropertyConverter(IPropertyConverter<TResult, TInput> converter)
        {
            _converter = converter;
        }

        Task<TResult> IPropertyConverter<TResult, Task<TInput>>.Convert<TMessage>(InitializeContext<TMessage> context, Task<TInput> input)
        {
            if (input == null)
                return default;

            if (input.IsCompleted)
                return _converter.Convert(context, input.Result);

            return ConvertInputAsync(context, input);
        }

        async Task<TResult> ConvertInputAsync<TMessage>(InitializeContext<TMessage> context, Task<TInput> inputTask)
            where TMessage : class
        {
            var inputValue = await inputTask.ConfigureAwait(false);

            return await _converter.Convert(context, inputValue);
        }
    }
}
