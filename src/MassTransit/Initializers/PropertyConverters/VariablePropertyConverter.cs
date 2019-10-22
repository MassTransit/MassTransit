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

            async Task<TResult> ConvertAsync()
            {
                var value = await inputTask.ConfigureAwait(false);

                var convertTask = _propertyConverter.Convert(context, value);
                if (convertTask.IsCompleted)
                    return convertTask.Result;

                return await convertTask.ConfigureAwait(false);
            }

            return ConvertAsync();
        }
    }
}
