namespace MassTransit.Initializers.TypeConverters
{
    public class ToNullableTypeConverter<T> :
        ITypeConverter<T?, T>
        where T : struct
    {
        public bool TryConvert(T input, out T? result)
        {
            result = input;
            return true;
        }
    }


    public class ToNullableTypeConverter<T, TInput> :
        ITypeConverter<T?, TInput>
        where T : struct
    {
        readonly ITypeConverter<T, TInput> _typeConverter;

        public ToNullableTypeConverter(ITypeConverter<T, TInput> typeConverter)
        {
            _typeConverter = typeConverter;
        }

        public bool TryConvert(TInput input, out T? result)
        {
            if (_typeConverter.TryConvert(input, out var intermediateValue))
            {
                result = intermediateValue;
                return true;
            }

            result = default;
            return false;
        }
    }
}
