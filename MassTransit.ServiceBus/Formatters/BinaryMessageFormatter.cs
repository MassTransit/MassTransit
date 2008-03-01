/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Formatters
{
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class BinaryMessageFormatter
        : IMessageFormatter
    {
        private static readonly IFormatter _formatter = new BinaryFormatter();

        public IMessageBody Serialize(IEnvelope env, IMessageBody body)
        {
            _formatter.Serialize(body.BodyStream, env.Messages);

            return body;
        }

        public IEnvelope Deserialize(IMessageBody messageBody)
        {
            IMessage[] messages = _formatter.Deserialize(messageBody.BodyStream) as IMessage[];

            return new Envelope(messages);
        }
    }
}