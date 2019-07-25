namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Variables;


    public class ConvertTypeVariablePropertyConverter<TResult, TInput, T> :
        IPropertyConverter<TResult, TInput>
        where TInput : class, IInitializerVariable<T>
    {
        readonly ITypeConverter<TResult, T> _typeConverter;

        public ConvertTypeVariablePropertyConverter(ITypeConverter<TResult, T> typeConverter)
        {
            _typeConverter = typeConverter;
        }

        public Task<TResult> Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
            where TMessage : class
        {
            if (input == default)
                return default;

            Task<T> inputTask = input.GetValue(context);
            if (inputTask.IsCompleted)
                return Task.FromResult(_typeConverter.TryConvert(inputTask.Result, out var result) ? result : default);

            return ConvertAsync(inputTask);
        }

        async Task<TResult> ConvertAsync(Task<T> inputTask)
        {
            var value = await inputTask.ConfigureAwait(false);

            return _typeConverter.TryConvert(value, out var result) ? result : default;
        }
    }
}
