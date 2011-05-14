// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    ///   The class used to deserialize a message serialized in Xml
    ///   Used since we don't know the type of the message until after the
    ///   MessageType property has been evaluated
    /// </summary>
    [XmlRoot(ElementName = "MessageEnvelope")]
    public class XmlReceiveMessageEnvelope :
        MessageEnvelopeBase
    {
        [XmlAnyElement]
        public XmlNode Message { get; set; }
    }
}