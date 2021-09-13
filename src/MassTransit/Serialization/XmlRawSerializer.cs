using MassTransit.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;

namespace MassTransit.Serialization
{
    public class XmlRawSerializer: IMessageSerializer
    {
        //Content Header Type of XML 
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+xml";
    public static readonly ContentType XmlContentType = new ContentType(ContentTypeHeaderValue);

    static readonly Lazy<Encoding> _encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true),
        LazyThreadSafetyMode.PublicationOnly);

    static readonly Lazy<XmlSerializer> _xmlSerializer;

    readonly RawXmlSerializerOptions _options;

    public XmlRawSerializer()
    {

    }
    static XmlRawSerializer()
    {
        _xmlSerializer = new Lazy<XmlSerializer>();
    }

    public static XmlSerializer XmlSerializer => new XmlSerializer(typeof(object));

    public ContentType ContentType => XmlContentType;

    public void Serialize<T>(Stream stream, SendContext<T> context) where T : class
    {
        try
        {
            context.ContentType = XmlContentType;

            if (_options.HasFlag(RawXmlSerializerOptions.AddTransportHeaders))
                SetRawXMLMessageHeaders<T>(context);
            var serializer = new XmlSerializer(typeof(T));
            var envelope = new XmlMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

            var encoding = new Lazy<Encoding>(() => new UTF8Encoding(false, true), LazyThreadSafetyMode.PublicationOnly);

            using (var writer = new StreamWriter(stream, encoding.Value, 1024, true))
            {

                serializer.Serialize(writer, envelope.Message);
                writer.Flush();

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



    static void SetRawXMLMessageHeaders<T>(SendContext context)
        where T : class
    {
        if (context.MessageId.HasValue)
            context.Headers.Set(MessageHeaders.MessageId, context.MessageId.Value.ToString());

        if (context.CorrelationId.HasValue)
            context.Headers.Set(MessageHeaders.CorrelationId, context.CorrelationId.Value.ToString());

        if (context.ConversationId.HasValue)
            context.Headers.Set(MessageHeaders.ConversationId, context.ConversationId.Value.ToString());

        context.Headers.Set(MessageHeaders.MessageType, string.Join(";", TypeMetadataCache<T>.MessageTypeNames));

        if (context.ResponseAddress != null)
            context.Headers.Set(MessageHeaders.ResponseAddress, context.ResponseAddress);

        if (context.FaultAddress != null)
            context.Headers.Set(MessageHeaders.FaultAddress, context.FaultAddress);

        if (context.InitiatorId.HasValue)
            context.Headers.Set(MessageHeaders.InitiatorId, context.InitiatorId.Value.ToString());

        if (context.SourceAddress != null)
            context.Headers.Set(MessageHeaders.SourceAddress, context.SourceAddress);
        context.Headers.Set(MessageHeaders.ContentType, ContentTypeHeaderValue);

        using (var writer = new StringWriter())
        {
            using (var xmlWriter = XmlWriter.Create(writer))
            {
                XmlSerializer.Serialize(writer, HostMetadataCache.Host);
                context.Headers.Set(MessageHeaders.Host.Info, writer.ToString());

            }
        }

    }
}
}
