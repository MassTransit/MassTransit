namespace MassTransit.Tests.Serialization.Approach
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using Magnum.Monads;

	public interface IObjectSerializer
	{
		IEnumerable<K<Action<XmlWriter>>> GetSerializationActions(ISerializerContext context, string localName, object value);
	}
}