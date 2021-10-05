using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace SendActiveMQTest
{
    public static class XmlExtensions
    {
        /// <summary>
        /// Converts an object to its serialized XML format.
        /// </summary>
        /// <typeparam name="T">The type of object we are operating on</typeparam>
        /// <param name="value">The object we are operating on</param>
        /// <param name="removeDefaultXmlNamespaces">Whether or not to remove the default XML namespaces from the output</param>
        /// <param name="omitXmlDeclaration">Whether or not to omit the XML declaration from the output</param>
        /// <param name="encoding">The character encoding to use</param>
        /// <returns>The XML string representation of the object</returns>
        public static string ToXmlString<T>(this T value, bool removeDefaultXmlNamespaces = true, bool omitXmlDeclaration = true, Encoding encoding = null) where T : class
        {
            XmlSerializerNamespaces namespaces = removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null;

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.CheckCharacters = false;

            using (var stream = new StringWriterWithEncoding(Encoding.UTF8))
            using (var writer = XmlWriter.Create(stream))
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value);
                return stream.ToString();
            }
        }
    }
}
