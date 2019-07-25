namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Util;
    using Variables;


    public class ConvertVariablePropertyConverter<TResult, TInput, T> :
        IPropertyConverter<TResult, TInput>
        where TInput : class, IInitializerVariable<T>
    {
        readonly IPropertyConverter<TResult, T> _propertyConverter;

        public ConvertVariablePropertyConverter(IPropertyConverter<TResult, T> propertyConverter)
        {
            _propertyConverter = propertyConverter;
        }

        public Task<TResult> Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
            where TMessage : class
        {
            if (input == default)
                return default;

            Task<T> inputTask = input.GetValue(context);
            if (inputTask.IsCompleted)
                return _propertyConverter.Convert(context, inputTask.Result);

            return ConvertAsync(context, inputTask);
        }

        async Task<TResult> ConvertAsync<TMessage>(InitializeContext<TMessage> context, Task<T> inputTask)
            where TMessage : class
        {
            var value = await inputTask.ConfigureAwait(false);

            return await _propertyConverter.Convert(context, value).ConfigureAwait(false);
        }
    }


    public class ConvertVariablePropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult, TInput>
        where TInput : class, IInitializerVariable<TResult>
    {
        public Task<TResult> Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
            where TMessage : class
        {
            return input?.GetValue(context) ?? TaskUtil.Default<TResult>();
        }
    }
}
