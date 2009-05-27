namespace MassTransit.Serialization.Custom.TypeSerializers
{
	using System;
	using System.Xml;

	public class UriSerializer :
		SerializerBase<Uri>
	{
		protected override void WriteValue(XmlWriter writer, object value)
		{
			writer.WriteValue(value.ToString());
		}
	}
}