using GreenPipes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace MassTransit.Serialization
{
    public class RawXmlMessageDeserializer
 :
        IMessageDeserializer
    {
        public RawXmlMessageDeserializer()
        {

        }

        //Content Header Type of XML 
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+xml";
        public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);
        public ContentType ContentType => XmlContentType;


        readonly XmlSerializer _deserializer;
        readonly RawXmlSerializerOptions _options;


        public RawXmlMessageDeserializer(XmlSerializer deserializer, RawXmlSerializerOptions options = RawXmlSerializerOptions.Default)
        {
            _deserializer = deserializer;
            _options = options;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("xml");
            scope.Add("contentType", XmlRawSerializer.XmlContentType.MediaType);
        }

        ContentType IMessageDeserializer.ContentType => XmlRawSerializer.XmlContentType;

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            try
            {
                var messageEncoding = GetMessageEncoding(receiveContext);

                using (var body = receiveContext.GetBodyStream())
                {
                    using (var reader = new StreamReader(body, messageEncoding, false, 1024, true))
                    {
                        var messageStr = reader.ReadToEnd();
                        return new RawXmlConsumeContext(_deserializer, receiveContext, messageStr, _options);
                    }
                }

                // using var jsonReader = new JsonTextReader(reader);


            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException("A JSON serialization exception occurred while deserializing the message", ex);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An exception occurred while deserializing the message", ex);
            }
        }

        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }


    }
}
