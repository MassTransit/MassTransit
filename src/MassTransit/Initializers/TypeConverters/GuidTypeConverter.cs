namespace MassTransit.Initializers.TypeConverters
{
    using System;


    public class GuidTypeConverter :
        ITypeConverter<string, Guid>,
        ITypeConverter<Guid, string>,
        ITypeConverter<Guid, NewId>,
        ITypeConverter<Guid, object>
    {
        public bool TryConvert(string input, out Guid result)
        {
            return Guid.TryParse(input, out result);
        }

        public bool TryConvert(NewId input, out Guid result)
        {
            result = input.ToGuid();

            return true;
        }

        public bool TryConvert(Guid input, out string result)
        {
            result = input.ToString("D");
            return true;
        }

        public bool TryConvert(object input, out Guid result)
        {
            return Guid.TryParse(input?.ToString(),out result);
        }
    }
}
