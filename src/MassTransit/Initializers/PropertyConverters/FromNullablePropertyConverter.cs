namespace MassTransit.Initializers.PropertyConverters
{
    using System.Threading.Tasks;


    public class FromNullablePropertyConverter<T, TInput> :
        IPropertyConverter<T, TInput?>
        where TInput : struct
    {
        readonly IPropertyConverter<T, TInput> _converter;

        public FromNullablePropertyConverter(IPropertyConverter<T, TInput> converter)
        {
            _converter = converter;
        }

        public Task<T> Convert<TMessage>(InitializeContext<TMessage> context, TInput? input)
            where TMessage : class
        {
            return _converter.Convert(context, input ?? default);
        }
    }
}