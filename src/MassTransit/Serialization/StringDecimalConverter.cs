// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Serialization
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    public class StringDecimalConverter :
        JsonConverter
    {
        const NumberStyles StringDecimalStyle =
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign |
            NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | 
            NumberStyles.AllowExponent;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text = Convert.ToString(value);
            if (string.IsNullOrEmpty(text))
                text = "";

            writer.WriteValue(text);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default(decimal);

            if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
                return Convert.ToDecimal(reader.Value, CultureInfo.InvariantCulture);

            Decimal result;
            if (reader.TokenType == JsonToken.String &&
                decimal.TryParse((string) reader.Value,StringDecimalStyle,CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            throw new JsonReaderException(string.Format(CultureInfo.InvariantCulture,
                "Error reading decimal. Expected a number but got {0}.", new object[] {reader.TokenType}));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (decimal) || objectType == typeof (decimal?);
        }
    }
}