// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Util;


    public class XmlMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

        static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true),
            LazyThreadSafetyMode.PublicationOnly);

        static readonly Lazy<JsonSerializer> _xmlSerializer;

        public static readonly JsonSerializerSettings XmlSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new List<JsonConverter>(new[]
            {
                new XmlNodeConverter
                {
                    DeserializeRootElementName = "envelope",
                    WriteArrayAttribute = false,
                    OmitRootObject = true,
                }
            })
        };

        static XmlMessageSerializer()
        {
            _xmlSerializer = new Lazy<JsonSerializer>(() => JsonSerializer.Create(XmlSerializerSettings));
        }

        public static JsonSerializer XmlSerializer => _xmlSerializer.Value;

        public ContentType ContentType => XmlContentType;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            try
            {
                context.ContentType = XmlContentType;

                var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

                var json = new StringBuilder(1024);

                using (var stringWriter = new StringWriter(json, CultureInfo.InvariantCulture))
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;

                    JsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

                    jsonWriter.Flush();
                    stringWriter.Flush();
                }

                using (var stringReader = new StringReader(json.ToString()))
                using (var jsonReader = new JsonTextReader(stringReader))
                {
                    var document = (XDocument)XmlSerializer.Deserialize(jsonReader, typeof(XDocument));

                    using (var writer = new StreamWriter(stream, _encoding.Value, 1024, true))
                    using (var xmlWriter = XmlWriter.Create(writer))
                    {
                        document.WriteTo(xmlWriter);
                    }
                }
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize message", ex);
            }
        }
    }
}