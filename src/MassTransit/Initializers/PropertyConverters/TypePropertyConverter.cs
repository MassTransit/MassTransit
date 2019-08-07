namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// Calls the property type converter, returning either the result or default.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class TypePropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult, TInput>
    {
        readonly ITypeConverter<TResult, TInput> _converter;

        public TypePropertyConverter(ITypeConverter<TResult, TInput> converter)
        {
            _converter = converter;
        }

        Task<TResult> IPropertyConverter<TResult, TInput>.Convert<TMessage>(InitializeContext<TMessage> context, TInput input)
        {
            return _converter.TryConvert(input, out var result)
                ? Task.FromResult(result)
                : TaskUtil.Default<TResult>();
        }
    }
}
