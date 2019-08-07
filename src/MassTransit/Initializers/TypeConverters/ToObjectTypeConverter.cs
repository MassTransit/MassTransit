namespace MassTransit.Initializers.TypeConverters
{
    public class ToObjectTypeConverter<T> :
        ITypeConverter<object, T>
    {
        public bool TryConvert(T input, out object result)
        {
            result = input;
            return true;
        }
    }
}