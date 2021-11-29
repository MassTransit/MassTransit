namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;


    public class StringDecimalConverter :
        BaseJsonConverter
    {
        const NumberStyles StringDecimalStyle =
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign |
            NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands |
            NumberStyles.AllowExponent;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var text = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(text))
                writer.WriteNull();
            else
                writer.WriteValue(text);
        }

        protected override IConverter ValueFactory(Type objectType)
        {
            if (objectType == typeof(decimal) || objectType == typeof(decimal?))
                return new CachedConverter();

            return new Unsupported();
        }


        class CachedConverter :
            IConverter
        {
            object IConverter.Deserialize(JsonReader reader, Type objectType, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return objectType == typeof(decimal) ? default(decimal) : default(decimal?);

                if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
                    return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);

                if (reader.TokenType == JsonToken.String)
                {
                    var text = (string)reader.Value;

                    if (string.IsNullOrWhiteSpace(text))
                        return objectType == typeof(decimal) ? default(decimal) : default(decimal?);

                    if (decimal.TryParse(text, StringDecimalStyle, CultureInfo.InvariantCulture, out var result))
                        return result;
                }

                throw new JsonReaderException($"Error reading decimal. Expected a number but got {reader.TokenType}.");
            }

            public bool IsSupported => true;
        }
    }
}
