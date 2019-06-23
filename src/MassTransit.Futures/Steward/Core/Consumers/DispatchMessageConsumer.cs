// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Steward.Core.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Contracts;
    using Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serialization;


    public class DispatchMessageConsumer :
        IConsumer<DispatchMessage>
    {
        readonly DispatchAgent _agent;

        public DispatchMessageConsumer(DispatchAgent agent)
        {
            _agent = agent;
        }

        public async Task Consume(ConsumeContext<DispatchMessage> context)
        {

            string body = Encoding.UTF8.GetString(context.ReceiveContext.GetBody());

            if (string.Compare(context.ReceiveContext.ContentType.MediaType, JsonMessageSerializer.JsonContentType.MediaType,
                StringComparison.OrdinalIgnoreCase)
                == 0)
                body = TranslateJsonBody(body, context.Message.Destination.ToString());
            else if (string.Compare(context.ReceiveContext.ContentType.MediaType, XmlMessageSerializer.XmlContentType.MediaType,
                StringComparison.OrdinalIgnoreCase) == 0)
                body = TranslateXmlBody(body, context.Message.Destination.ToString());
            else
                throw new InvalidOperationException("Only JSON and XML messages can be scheduled");

            var dispatchContext = new ConsumerDispatchContext(context, body);

            await _agent.Execute(dispatchContext);
        }

        static string TranslateJsonBody(string body, string destination)
        {
            JObject envelope = JObject.Parse(body);

            envelope["destinationAddress"] = destination;

            JToken message = envelope["message"];

            JToken payload = message["payload"];
            JToken payloadType = message["payloadType"];

            envelope["message"] = payload;
            envelope["messageType"] = payloadType;

            return JsonConvert.SerializeObject(envelope, Formatting.Indented);
        }

        static string TranslateXmlBody(string body, string destination)
        {
            using (var reader = new StringReader(body))
            {
                XDocument document = XDocument.Load(reader);

                XElement envelope = (from e in document.Descendants("envelope") select e).Single();

                XElement destinationAddress = (from a in envelope.Descendants("destinationAddress") select a).Single();

                XElement message = (from m in envelope.Descendants("message") select m).Single();
                IEnumerable<XElement> messageType = (from mt in envelope.Descendants("messageType") select mt);

                XElement payload = (from p in message.Descendants("payload") select p).Single();
                IEnumerable<XElement> payloadType = (from pt in message.Descendants("payloadType") select pt);

                message.Remove();
                messageType.Remove();

                destinationAddress.Value = destination;

                message = new XElement("message");
                message.Add(payload.Descendants());
                envelope.Add(message);

                envelope.Add(payloadType.Select(x => new XElement("messageType", x.Value)));

                return document.ToString(SaveOptions.DisableFormatting);
            }
        }
    }
}
