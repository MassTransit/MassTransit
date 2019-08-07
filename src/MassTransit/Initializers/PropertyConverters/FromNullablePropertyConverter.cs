namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    public class FromNullablePropertyConverter<TResult> :
        IPropertyConverter<TResult, TResult?>
        where TResult : struct
    {
        Task<TResult> IPropertyConverter<TResult, TResult?>.Convert<T>(InitializeContext<T> context, TResult? input)
        {
            return Task.FromResult(input ?? default);
        }
    }


    public class FromNullablePropertyConverter<TResult, TInput> :
        IPropertyConverter<TResult, TInput?>
        where TInput : struct
    {
        readonly IPropertyConverter<TResult, TInput> _converter;

        public FromNullablePropertyConverter(IPropertyConverter<TResult, TInput> converter)
        {
            _converter = converter;
        }

        Task<TResult> IPropertyConverter<TResult, TInput?>.Convert<T>(InitializeContext<T> context, TInput? input)
        {
            return _converter.Convert(context, input ?? default);
        }
    }
}
