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

        public async Task<TResult> Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
            where TMessage : class
        {
            if (input == default)
                return default;

            var intermediateResult = await input.GetValue(context).ConfigureAwait(false);

            return await _propertyConverter.Convert(context, intermediateResult).ConfigureAwait(false);
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
