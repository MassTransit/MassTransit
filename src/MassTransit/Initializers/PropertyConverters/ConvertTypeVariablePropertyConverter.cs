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

        public async Task<TResult> Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
            where TMessage : class
        {
            if (input == default)
                return default;

            var intermediateResult = await input.GetValue(context).ConfigureAwait(false);

            return _typeConverter.TryConvert(intermediateResult, out var result)
                ? result
                : default;
        }
    }
}