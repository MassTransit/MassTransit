namespace MassTransit.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mime;
    using System.Text;
    using System.Xml.Linq;


    public class ForwardXmlMessageSerializer :
        IMessageSerializer
    {
        readonly XDocument _document;
        readonly Encoding _encoding;
        readonly ReceiveContext _receiveContext;

        public ForwardXmlMessageSerializer(ReceiveContext receiveContext)
        {
            _receiveContext = receiveContext;
            _encoding = GetMessageEncoding(receiveContext);

            _document = GetXmlDocument(receiveContext, _encoding);
        }

        public ContentType ContentType => _receiveContext.ContentType;

        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            SetDestinationAddress(context.DestinationAddress?.ToString());
            UpdateHeaders(context.Headers);

            var body = _document.ToString(SaveOptions.DisableFormatting);

            var bytes = _encoding.GetBytes(body);

            stream.Write(bytes, 0, bytes.Length);
        }

        void SetDestinationAddress(string value)
        {
            var envelope = (from e in _document.Descendants("envelope") select e).Single();

            var destinationAddress = (from a in envelope.Descendants("destinationAddress") select a).Single();

            destinationAddress.Value = value;
        }

        void UpdateHeaders(Headers values)
        {
            var envelope = (from e in _document.Descendants("envelope") select e).Single();

            var headers = (from h in envelope.Descendants("headers") select h).SingleOrDefault();
            if (headers == null)
            {
                headers = new XElement("headers");
                envelope.Add(headers);
            }

            foreach (KeyValuePair<string, object> header in values.GetAll())
                headers.Add(new XElement(header.Key, header.Value));
        }

        static XDocument GetXmlDocument(ReceiveContext receiveContext, Encoding encoding)
        {
            var bodyString = encoding.GetString(receiveContext.GetBody());

            using var reader = new StringReader(bodyString);

            var document = XDocument.Load(reader);

            return document;
        }

        static Encoding GetMessageEncoding(ReceiveContext receiveContext)
        {
            var contentEncoding = receiveContext.TransportHeaders.Get("Content-Encoding", default(string));

            return string.IsNullOrWhiteSpace(contentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(contentEncoding);
        }
    }
}
