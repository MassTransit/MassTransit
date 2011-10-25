// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Serialization.Custom
{
	using System;
	using System.Collections.Generic;
	using System.Xml;

    /// <summary>
	/// The serializer context is passed through all of the serializers to provide a central context for
	/// dispatching to the appropriate serializer for each type encountered without using a static class
	/// </summary>
	public interface ISerializerContext
	{
		/// <summary>
		/// Returns the namespace prefix to use for the specified element
		/// </summary>
		/// <param name="localName">The name of the element being added to the XML document</param>
		/// <param name="ns">The namespace that defines the type for the element</param>
		/// <returns>A prefix to use when writing the element to the XML stream</returns>
		string GetPrefix(string localName, string ns);

		/// <summary>
		/// Returns the type for a property if the property type is to be overridden by the serializer
		/// </summary>
		/// <param name="declaringType">The type containing the property</param>
		/// <param name="propertyType">The type of the property</param>
		/// <param name="value">The value to use to obtain the type, if necessary</param>
		/// <returns>The property type, or the overriding type</returns>
		Type MapType(Type declaringType, Type propertyType, object value);

		/// <summary>
		/// Writes any namespace information that was collected to the document element attribute
		/// </summary>
		/// <param name="writer">The XmlWriter to use for writing the attributes</param>
		void WriteNamespaceInformationToXml(XmlWriter writer);

		/// <summary>
		/// Delegates the serialization of an obj
		/// </summary>
		/// <param name="localName">The name of the element in the XML document</param>
		/// <param name="type">The type of object to serialize</param>
		/// <param name="value">The actual object to serialize</param>
		/// <returns>An enumeration of continuations to actually write the XML</returns>
		IEnumerable<Continuation<Action<XmlWriter>>> SerializeObject(string localName, Type type, object value);
	}
}