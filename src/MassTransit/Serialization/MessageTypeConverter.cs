namespace MassTransit.Serialization
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public class MessageTypeConverter :
		JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("This converter should not be used for writing as it can create loops");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
			{
				return new List<string> { (string)reader.Value };
			}

			var result = (string[]) serializer.Deserialize(reader, typeof (string[]));

			return new List<string>(result);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IList<string>) || objectType == typeof(List<string>);
		}
	}
}