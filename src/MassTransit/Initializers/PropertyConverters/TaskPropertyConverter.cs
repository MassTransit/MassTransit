namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Converts a <see cref="Task{T}"/> to {T} by awaiting the result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class TaskPropertyConverter<TResult> :
        IPropertyConverter<TResult, Task<TResult>>,
        IPropertyConverter<Task<TResult>, TResult>
    {
        Task<TResult> IPropertyConverter<TResult, Task<TResult>>.Convert<T>(InitializeContext<T> context, Task<TResult> input)
        {
            return input;
        }

        public Task<Task<TResult>> Convert<TMessage>(InitializeContext<TMessage> context, TResult input)
            where TMessage : class
        {
            return Task.FromResult(Task.FromResult(input));
        }
    }


    /// <summary>
    /// Converts a <see cref="Task{T}"/> to {T} by awaiting the result
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class TaskPropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult, Task<TInput>>,
        IPropertyConverter<Task<TResult>, TInput>
    {
        readonly IPropertyConverter<TResult, TInput> _converter;

        public TaskPropertyConverter(IPropertyConverter<TResult, TInput> converter)
        {
            _converter = converter;
        }

        Task<TResult> IPropertyConverter<TResult, Task<TInput>>.Convert<T>(InitializeContext<T> context, Task<TInput> input)
        {
            if (input == default)
                return TaskUtil.Default<TResult>();

            if (input.IsCompleted)
                return _converter.Convert(context, input.Result);

            async Task<TResult> ConvertAsync()
            {
                var value = await input.ConfigureAwait(false);

                var convertTask = _converter.Convert(context, value);
                if (convertTask.IsCompleted)
                    return convertTask.Result;

                return await convertTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }

        public Task<Task<TResult>> Convert<T>(InitializeContext<T> context, TInput input)
            where T : class
        {
            return Task.FromResult(_converter.Convert(context, input));
        }
    }
}
