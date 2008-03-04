using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MassTransit.ServiceBus.Formatters
{
    using System.Collections.Generic;
    using System.Xml;

    public class XmlMessageFormatter :
        IMessageFormatter
    {
        private Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();

        public XmlMessageFormatter()
        {
            List<Type> types = MessageFinder.FindAll();
            foreach(Type t in types)
            {
                _serializers.Add(t, new XmlSerializer(t));
            }
        }


        public void Serialize(IFormattedBody body, params IMessage[] messages)
        {
            MemoryStream mems = new MemoryStream();
            _serializers[messages[0].GetType()].Serialize(mems, messages[0]);

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
            
            object o = _serializers[Type.GetType("PingMessage", true, true)].Deserialize(mems);

            return o as IMessage[];
        }
    }
}