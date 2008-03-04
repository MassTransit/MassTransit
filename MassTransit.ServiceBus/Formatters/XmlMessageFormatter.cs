using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MassTransit.ServiceBus.Formatters
{
    using System.Xml;

    public class XmlMessageFormatter :
        IMessageFormatter
    {
        private XmlSerializer _serializer;


        public XmlMessageFormatter(Type type)
        {
            _serializer = new XmlSerializer(type);
        }
        public XmlMessageFormatter(Type type, Type[] types)
        {
            _serializer = new XmlSerializer(type, types);
        }


        public void Serialize(IFormattedBody body, params IMessage[] messages)
        {
            MemoryStream mems = new MemoryStream();
            _serializer.Serialize(mems, messages[0]);

            byte[] buffer = new byte[mems.Length];
            mems.Position = 0;
            mems.Read(buffer, 0, buffer.Length);

            body.Body = Encoding.Default.GetString(buffer);
        }

        public IMessage[] Deserialize(IFormattedBody formattedBody)
        {
            MemoryStream mems = new MemoryStream();
            string body = formattedBody.Body.ToString();
            byte[] buffer = new byte[body.Length];
            buffer = Encoding.Default.GetBytes(body);
            mems.Write(buffer, 0, buffer.Length);
            
            object o = _serializer.Deserialize(mems);

            return o as IMessage[];
        }
    }
}