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
                if(!_serializers.ContainsKey(t))
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
            string body = formattedBody.Body.ToString();

            StringReader sr = new StringReader(body);

            XmlReader xmlReader = XmlReader.Create(sr);

            object result = null;

            foreach (KeyValuePair<Type, XmlSerializer> pair in _serializers)
            {
                if(pair.Value.CanDeserialize(xmlReader))
                {
                    result = pair.Value.Deserialize(xmlReader);
                    break;
                }
            }

            return new IMessage[] {result as IMessage};
        }
    }
}