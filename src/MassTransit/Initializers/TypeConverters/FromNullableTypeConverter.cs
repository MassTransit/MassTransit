namespace MassTransit.Initializers.TypeConverters
{
    public class FromNullableTypeConverter<T> :
        ITypeConverter<T, T?>
        where T : struct
    {
        public bool TryConvert(T? input, out T result)
        {
            result = input ?? default;
            return true;
        }
    }


    public class FromNullableTypeConverter<T, TInput> :
        ITypeConverter<T, TInput?>
        where TInput : struct
    {
        readonly ITypeConverter<T, TInput> _typeConverter;

        public FromNullableTypeConverter(ITypeConverter<T, TInput> typeConverter)
        {
            _typeConverter = typeConverter;
        }

        public bool TryConvert(TInput? input, out T result)
        {
            return _typeConverter.TryConvert(input ?? default, out result);
        }
    }
}
