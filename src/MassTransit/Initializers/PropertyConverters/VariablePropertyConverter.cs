namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Util;


    public class VariablePropertyConverter<TResult, TVariable> :
        IPropertyConverter<TResult, TVariable>
        where TVariable : class, IInitializerVariable<TResult>
    {
        public Task<TResult> Convert<T>(InitializeContext<T> context, TVariable input)
            where T : class
        {
            return input?.GetValue(context) ?? TaskUtil.Default<TResult>();
        }
    }


    public class VariablePropertyConverter<TResult, TVariable, TValue> :
        IPropertyConverter<TResult, TVariable>
        where TVariable : class, IInitializerVariable<TValue>
    {
        readonly IPropertyConverter<TResult, TValue> _propertyConverter;

        public VariablePropertyConverter(IPropertyConverter<TResult, TValue> propertyConverter)
        {
            _propertyConverter = propertyConverter;
        }

        public Task<TResult> Convert<T>(InitializeContext<T> context, TVariable input)
            where T : class
        {
            if (input == default)
                return default;

            Task<TValue> inputTask = input.GetValue(context);
            if (inputTask.IsCompleted)
                return _propertyConverter.Convert(context, inputTask.Result);

            return ConvertAsync(context, inputTask);
        }

        async Task<TResult> ConvertAsync<TMessage>(InitializeContext<TMessage> context, Task<TValue> inputTask)
            where TMessage : class
        {
            var value = await inputTask.ConfigureAwait(false);

            return await _propertyConverter.Convert(context, value).ConfigureAwait(false);
        }
    }
}
