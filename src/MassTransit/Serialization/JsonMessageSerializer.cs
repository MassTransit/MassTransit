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
    using System;
    using System.IO;
    using System.Text;
    using Internal;
    using log4net;
    using Magnum.Extensions;
    using Newtonsoft.Json;

    public class JsonMessageSerializer :
        IMessageSerializer
    {
        static readonly ILog _log = LogManager.GetLogger(typeof (JsonMessageSerializer));

        public void Serialize<T>(Stream output, T message)
        {
            var data = JsonConvert.SerializeObject(message);
            var envelope = JsonMessageEnvelope.Create<T>(data);
            var strOut = JsonConvert.SerializeObject(envelope);
            var buff = Encoding.UTF8.GetBytes(strOut);

            output.Write(buff, 0, buff.Length);
        }

        public object Deserialize(Stream input)
        {
            var text = input.ReadToEndAsText();
            var env = JsonConvert.DeserializeObject<JsonMessageEnvelope>(text);

            InboundMessageHeaders.SetCurrent(env.GetMessageHeadersSetAction());

            var data = env.Message;
            var mtype = Type.GetType(env.MessageType);
            return JsonConvert.DeserializeObject(data, mtype);
        }
    }
}