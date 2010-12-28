// Copyright 2007-2010 The Apache Software Foundation.
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
    using System.IO;
    using System.Runtime.Serialization;
    using Custom;
    using Internal;
    using log4net;

    public class CustomXmlMessageSerializer :
        IMessageSerializer
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (CustomXmlMessageSerializer));

        static readonly CustomXmlSerializer _serializer = new CustomXmlSerializer();

        public CustomXmlMessageSerializer()
        {
            Namespace = "http://tempuri.org/";
        }

        /// <summary>
        ///   The namespace to place in outgoing XML.
        /// </summary>
        public string Namespace { get; set; }

        public void Serialize<T>(Stream stream, T message)
        {
            var envelope = XmlMessageEnvelope.Create(message);

            _serializer.Serialize(stream, envelope);
        }

        public object Deserialize(Stream stream)
        {
            object message = _serializer.Deserialize(stream);

            if (message == null)
                throw new SerializationException("Could not deserialize message.");

            if (message is XmlMessageEnvelope)
            {
                XmlMessageEnvelope envelope = message as XmlMessageEnvelope;

                InboundMessageHeaders.SetCurrent(envelope.GetMessageHeadersSetAction());

                return envelope.Message;
            }

            return message;
        }
    }
}