namespace MassTransit.Initializers.TypeConverters
{
    public class StateTypeConverter :
        ITypeConverter<string, State>
    {
        public bool TryConvert(State input, out string result)
        {
            if (input != null)
            {
                result = input.Name;
                return true;
            }

            result = null;
            return false;
        }
    }
}
