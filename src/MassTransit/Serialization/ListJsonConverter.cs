namespace MassTransit.Serialization
{
	using System;
	using System.Collections.Generic;
	using Magnum.Reflection;
	using Newtonsoft.Json;

	public class ListJsonConverter :
		JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("This converter should not be used for writing as it can create loops");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			Type elementType = objectType.GetGenericArguments()[0];

			return this.FastInvoke<ListJsonConverter, object>(new[] {elementType}, "GetSingleItemList", reader, serializer);
		}

		object GetSingleItemList<T>(JsonReader reader, JsonSerializer serializer)
		{
			var list = new List<T>();

			if (reader.TokenType == JsonToken.StartArray)
			{
				serializer.Populate(reader, list);

				return list;
			}

			T item = (T) serializer.Deserialize(reader, typeof (T));
			list.Add(item);

			return list;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGenericType &&
				(objectType.GetGenericTypeDefinition() == typeof(IList<>) || objectType.GetGenericTypeDefinition() == typeof(List<>));
		}
	}
}